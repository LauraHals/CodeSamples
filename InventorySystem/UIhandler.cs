using static GameSystems;
using UnityEngine;
using System.Collections.Generic;

public class UIhandler : MonoBehaviour
{
    public static UIhandler Instance;

    [SerializeField]
    private ShopData shopData;

    [SerializeField]
    private GameObject shopSlotPrefab;

    [SerializeField]
    private Transform shopSlotParent;

    [SerializeField]
    private GameObject inventorySlotPrefab;

    [SerializeField]
    private Transform inventorySlotParent;

    private List<InventoryItemSlot> inventorySlots = new List<InventoryItemSlot>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PopulateInventory();
        PopulateShop();
    }

    /// <summary>
    /// Spawns inventory-slots based on owned items
    /// </summary>
    private void PopulateInventory()
    {
        for (int i = 0; i < Save.ownedItems.Length; i++)
        {
            if(Save.ownedItems[i] < 1)
            {
                //Player does not own this item
                continue;
            }

            GameObject slot = Instantiate(inventorySlotPrefab, inventorySlotParent);
            if (slot.TryGetComponent(out InventoryItemSlot component))
            {
                component.InitializeSlot(GameSystems.Inventory.GetDataByID(i));
                inventorySlots.Add(component);
            }
            else
            {
                Debug.LogWarning($"{inventorySlotPrefab.name} does not contain InventoryItemSlot-component!");
            }

            slot.SetActive(true);
        }
    }

    /// <summary>
    /// Spawns shop-item-slots based on Shop-data
    /// </summary>
    private void PopulateShop()
    {
        foreach (ItemData item in shopData.itemsInShop)
        {
            GameObject slot = Instantiate(shopSlotPrefab, shopSlotParent);
            slot.GetComponent<ItemSlot>().InitializeSlot(item);
            slot.SetActive(true);
        }
    }

    /// <summary>
    /// Called when pressing item buy-button in shop
    /// </summary>
    /// <param name="item">ItemData in slot</param>
    public void TryBuyItem(ItemData item)
    {
        if (item.playerLevel > Save.playerLevel)
        {
            Debug.Log(Save.playerLevel.ToString());
            DoozyUIManager.ShowErrorMessage("Level too low");
        }
        else if (item.cost > Save.currency)
        {
            DoozyUIManager.ShowErrorMessage("Not enough currency");
        }
        else
        {
            //Bought item
            Save.currency -= item.cost;
            GameSystems.Inventory.AddItem(item);

        }
    }

    public void UpdateSlot(ItemData item)
    {
        foreach (InventoryItemSlot slot in inventorySlots)
        {
            if(slot.Data == item)
            {
                slot.UpdateUIAmount();
                return;
            }
        }

        //Spawn new slot if picked up item for first time
        GameObject newSlot = Instantiate(inventorySlotPrefab, inventorySlotParent);
        if (newSlot.TryGetComponent(out InventoryItemSlot component))
        {
            component.InitializeSlot(item);
            inventorySlots.Add(component);
        }
        else
        {
            Debug.LogWarning($"{inventorySlotPrefab.name} does not contain InventoryItemSlot-component!");
        }

        newSlot.SetActive(true);
    }
}
