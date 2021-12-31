using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameSystems;

public class InventoryItemSlot : ItemSlot
{
	public ItemData Data { get => itemData; }

	[SerializeField]
	private Image fillImage;

	[SerializeField]
	private TextMeshProUGUI stackTxt;

	private Coroutine currentTimer = null;
	private int stackAmount;

	public override void InitializeSlot(ItemData itemData)
	{
		base.InitializeSlot(itemData);
		UpdateUIAmount();
	}

	public void UpdateUIAmount()
	{
		stackAmount = GameSystems.Inventory.GetAmountOfOwned(itemData);
		stackTxt.text = stackAmount > 0 ? stackAmount.ToString() : "";
	}

	public override void OnClick()
	{
		// Wait until previous powerup has been used
		if (currentTimer != null || stackAmount < 1)
		{
			return;
		}

		itemData.Use();
		GameSystems.Inventory.RemoveItem(itemData);
		currentTimer = StartCoroutine(Timer(itemData.duration));
		UpdateUIAmount();
	}

	/// <summary>
	/// Lerps UI icon which represents how long item is active
	/// </summary>
	/// <param name="timeToFill">Time to complete fill animation</param>
	/// <returns>null</returns>
	protected IEnumerator Timer(float timeToFill)
	{
		float start = 1;
		float end = 0;
		float timer = 0;

		fillImage.fillAmount = 1;

		while (timer < timeToFill)
		{

			fillImage.fillAmount = Mathf.Lerp(start, end, timer / timeToFill);
			timer += Time.deltaTime;
			yield return null;
		}

		fillImage.fillAmount = end;

		currentTimer = null;
	}
}
