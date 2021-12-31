using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    public string itemName;
    [TextArea(1, 4)]
    public string description;
    public int cost;
    [Tooltip("Required level to unlock item")]
    public int playerLevel;
    public Sprite icon;
    [Tooltip("Remove item after using")]
    public bool singleUse = true;
    [Tooltip("Item duration or 'item used'-indicator duration ")]
    [Range(0.1f, 999f)]
    public float duration = 0.5f;

    public abstract void Use();

}
