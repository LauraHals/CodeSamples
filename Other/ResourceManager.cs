using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static GameSystems;

/// <summary>
/// Different resource-types that can be collected in game
/// </summary>
public enum ResourceType
{
    Coal,
    Iron,
    Silver,
    Gold,
    Diamond,
    Sapphire,
    Emerald,
    Crystal
}

/// <summary>
/// Handles different resources that player can collect
/// </summary>
public class ResourceManager : MonoBehaviour
{
    [Tooltip("Chance to get coal when mining")]
    [Range(0, 1)]
    public float luckPercentage = 0.20f;

    [Tooltip("How much value has one coal, silver, iron, gold, diamond, sapphire, emerald, crystal")]
    public int[] values = { 1, 7, 12, 20, 30, 50, 80, 100 };

    [Tooltip("Collected-text prefab")]
    public GameObject collectedText;

    public TextMeshProUGUI storageText;

    //Other text colors are based on texture, except coal which is not an object
    public Color coalColor;

    // Notify UI and others when multiplier changes
    public GameEvent resourceMultiplierChanged;

    public static ResourceManager instance;

    public Dictionary<ResourceType, int> collectedResources = new Dictionary<ResourceType, int>();
    public int ValueMultiplier => multiplier;

    private float currency;
    private int multiplier = 1;
    private List<Multiplier> multiplierQueue = new List<Multiplier>();
    private float randomValue;
    private int currentStorageRoom = 0;
    

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if(Save.backpack.Count > 0)
        {
            foreach (KeyValuePair<int, int> kvp in Save.backpack)
            {
                collectedResources.Add((ResourceType)kvp.Key, kvp.Value);

                if (kvp.Key != 0)
                {
                    currentStorageRoom += kvp.Value;
                }
            }

            DisplayCollected();
        }

        UpdateStorageText();

    }

    public void DisplayCollected()
    {
        DoozyUIManager.FormatResources(collectedResources);
    }

    public void UpdateStorageText()
    {
        if (storageText != null)
        {
            storageText.text = currentStorageRoom.ToString() + " / " + Save.upgrades[1].ToString();
        }
    }

    public void CollectedResource(ResourceType type, int amount, Color color, Vector3 position)
    {
        //backpack is full (don't include coals)
        if(currentStorageRoom >= Save.upgrades[1] && type != ResourceType.Coal)
        {
            DoozyUIManager.ShowTempPopup(DoozyUIManager.TempPopups.STORAGE);
            return;
        }

        // If resource type has already been collected, add to amount
        if (collectedResources.ContainsKey(type))
        {
            collectedResources[type] += amount * multiplier;
            Save.backpack[(int)type] += amount * multiplier;
        } else
        {
            // else add new resource type
            collectedResources.Add(type, amount * multiplier);
            Save.backpack.Add((int)type, amount * multiplier);
        }

        //Spawn collected text on worldspace canvas
        GameObject go = ObjectPooler.instance.GetPooledObject(collectedText.name);
        go.transform.SetParent(WorldSpaceCanvas.instance.transform);
        go.GetComponent<UIpickupText>().Initialize(type.ToString(), color, amount * multiplier);
        position.y += 1;
        go.transform.position = position;
        go.SetActive(true);
        DarkTonic.MasterAudio.MasterAudio.FireCustomEvent(AudioCustomEvents.CollectedResource, transform);
        XpBar.instance.Fill((int)type + 1);

        //Add to storage
        if(type != ResourceType.Coal)
        {
            currentStorageRoom++;
            if(currentStorageRoom == Save.upgrades[1])
            {
                DoozyUIManager.ShowTempPopup(DoozyUIManager.TempPopups.STORAGE);
            }

            UpdateStorageText();
        }
       
    }

    public void RandomResource()
    {
        randomValue = Random.Range(0.0f, 1.0f);

        if (randomValue <= luckPercentage)
        {
            CollectedResource(ResourceType.Coal, Random.Range(1, 10), coalColor, PlayerObj.playerInstance.transform.position);
            XpBar.instance.Fill(1);
        }

        Save.allTimeCollected[0] += (int) randomValue;
    }

    public void SellResources()
    {
        foreach (KeyValuePair<ResourceType, int> kvp in collectedResources)
        {
            currency += kvp.Value * values[(int) kvp.Key];
            Debug.Log(kvp.Key.ToString() + " value: " + kvp.Value.ToString());

            Save.allTimeCollected[(int) kvp.Key] += kvp.Value;
        }

  //      Not currently using clicker
  //      if(ClickerRunner.instance != null)
  //      {
  //          ClickerRunner.instance.Manager.AddCurrency(ClickerRunner.instance.defaultCurrency, currency);
  //      }
        
        collectedResources.Clear();
        Save.backpack.Clear();
        currentStorageRoom = 0;
        UpdateStorageText();
    }

    //ex. 2x multiplier = picking up 1 gold gives 2 gold.
    public void IncreaseMultiplier(int amount, float movesActive)
    {
        if(multiplier == 1)
        {
            amount--;
        }

        multiplier += amount;
        multiplierQueue.Add(new Multiplier(movesActive, amount));
        RaiseEvent(resourceMultiplierChanged);
    }

    private void DecreaseMultiplier(int amount)
    {
        if(multiplier == 1) { return; }

        multiplier -= amount;
        RaiseEvent(resourceMultiplierChanged);
    }

    public void ResetMultiplier()
    {
        multiplier = 1;
        RaiseEvent(resourceMultiplierChanged);
    }

    private void RaiseEvent(GameEvent _event)
    {
        if(_event != null)
        {
            _event.Raise();
        }
    }

    public void ReduceMultiplierActiveTimes()
    {
        if(multiplierQueue.Count == 0) { return; }

        int entries = multiplierQueue.Count -1;

        for (int i = entries; i >= 0; i--)
        {
            multiplierQueue[i].activeTime--;
            if(multiplierQueue[i].activeTime <= 0)
            {
                DecreaseMultiplier(multiplierQueue[i].multiplier);
                multiplierQueue.RemoveAt(i);
            }
        }
    }


}
