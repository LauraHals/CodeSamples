using UnityEngine;
using UnityEngine.UI;

public abstract class ItemSlot : MonoBehaviour
{
    [SerializeField]
    private Image icon;

    protected ItemData itemData;

    /// <summary>
    /// Assigns correct icon and data to slot-button
    /// </summary>
    /// <param name="itemData">ItemData in slot</param>
    public virtual void InitializeSlot(ItemData itemData)
    {
        this.itemData = itemData;
        icon.sprite = itemData.icon;
    }

    public abstract void OnClick();
}
