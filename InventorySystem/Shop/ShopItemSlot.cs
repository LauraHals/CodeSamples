public class ShopItemSlot : ItemSlot
{
    public override void OnClick()
    {
        if (UIhandler.Instance != null)
        {
            UIhandler.Instance.TryBuyItem(itemData);
        }
    }
}
