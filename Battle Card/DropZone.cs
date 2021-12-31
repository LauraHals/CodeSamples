using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    /// <summary>
    /// Return card to hand if it's droppped here instead of Game Board
    /// </summary>
    /// <param name="eventData">PointerEventData containing card</param>
    public void OnDrop(PointerEventData eventData)
    {
        BattleCardBase card = eventData.pointerDrag.GetComponent<BattleCardBase>();

        if (card != null)
        {
            card.returnToHand = true;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }
}
