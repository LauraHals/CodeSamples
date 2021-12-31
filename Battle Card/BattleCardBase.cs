using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Base class for battle cards. Ingame card initialization & drag 'n' drop.
/// </summary>
public abstract class BattleCardBase : MonoBehaviour, IDraggable, IClickable
{
    // Contains all data about card
    public BattleCardData data;

    //Displayed information about card
    #region CardDisplay
    [SerializeField]
    private Image cardArt;

    [SerializeField]
    private TextMeshProUGUI cardName;

    [SerializeField]
    private TextMeshProUGUI manaCost;

    [SerializeField]
    private TextMeshProUGUI lvl;

    [SerializeField]
    private TextMeshProUGUI damage;

    [SerializeField]
    private TextMeshProUGUI description;

    [SerializeField]
    private TextMeshProUGUI type;

    [SerializeField]
    private Image typeImg;
    #endregion

    public GameObject placeholderPrefab;
    public bool returnToHand;

    //Position before dragging starts
    protected Vector3 startPos;
    protected Transform originalParent;
    protected CanvasGroup canvasGroup;
    protected GameObject dummy;
    
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Initialize()
    {
        cardArt.sprite = data.cardArt;
        cardName.text = data.cardName;
        description.text = data.description;
        manaCost.text = data.manaCost.ToString();
        lvl.text = data.cardLvl.ToString();
        damage.text = data.ATK.ToString();
        type.text = data.type.ToString();
    }

    protected abstract bool IsPlayable();

    protected abstract void Cast();

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        dummy = Instantiate(placeholderPrefab, transform.position, Quaternion.identity, originalParent);
        dummy.transform.SetSiblingIndex(this.transform.GetSiblingIndex());
        dummy.SetActive(true);
        transform.SetParent(transform.root);
        eventData.pointerDrag = gameObject;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //If was dropped on board > play
        // else return to hand
        if (!returnToHand && IsPlayable())
        {
            Cast();
        }else
        {
            transform.SetParent(originalParent);
            canvasGroup.blocksRaycasts = true;
            returnToHand = false;
            transform.SetSiblingIndex(dummy.transform.GetSiblingIndex());
            Destroy(dummy);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
    }
}
