using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
