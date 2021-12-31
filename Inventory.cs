using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameSystems;

[CreateAssetMenu(menuName = "FF-Assets/Inventory")]
public class Inventory : ScriptableObject
{
	public int ItemDBcount => itemDatabase.Count;

	//All item datas
	[SerializeField]
	private List<ItemData> itemDatabase;

	// Current inventory (owned items)
	private int[] inventory;

	public void Initialize()
	{
		// Link inventory to current save-state
		inventory = Save.ownedItems;
	}

	/// <summary>
	/// Increases inventory-array's value by one at given index
	/// </summary>
	/// <param name="item">item to add</param>
   public void AddItem(ItemData item)
   {
		inventory[GetItemID(item)]++;
		if(UIhandler.Instance != null)
		{
			UIhandler.Instance.UpdateSlot(item);
		}
   }

	/// <summary>
	/// Remove used item from inventory
	/// </summary>
	/// <param name="item">item to remove</param>
	public void RemoveItem(ItemData item)
	{
		int id = GetItemID(item);

		if(id >= 0)
		{
			inventory[id]--;
			//Clamp amount to zero in case of error
			if (inventory[id] < 0)
			{
				inventory[id] = 0;
			}
		}
	}

	/// <summary>
	/// Checks item's index in itemDatabase, which contains all different game items
	/// </summary>
	/// <param name="item">Item to check</param>
	/// <returns>Index of item in itemDatabase</returns>
	public int GetItemID(ItemData item)
	{
		if (itemDatabase.Contains(item))
		{
			return itemDatabase.IndexOf(item);
		}
		else
		{
			Debug.LogWarning($"itemIDs does not contain item { item.name}");
			return -1;
		}
	}

	/// <summary>
	/// Checks the value in inventory-array at given index
	/// </summary>
	/// <param name="item">Item to check</param>
	/// <returns>Amount in stack</returns>
	public int GetAmountOfOwned(ItemData item)
	{
		return inventory[GetItemID(item)];
	}
	
	/// <summary>
	/// Returns correct ItemData based on ID in game save
	/// </summary>
	/// <param name="id">ID of item</param>
	/// <returns>Item's data</returns>
	public ItemData GetDataByID(int id)
	{
		if(itemDatabase.Count > id)
		{
			return itemDatabase[id];
		}
		else
		{
			Debug.LogWarning($"itemDatabase has only {itemDatabase.Count} entries. ID: {id} ");
			return null;
		}
	}
}
