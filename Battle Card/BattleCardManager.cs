using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Spawns cards to player's hand 
/// </summary>
public class BattleCardManager : MonoBehaviour
{
    #region SerializedFields
    [SerializeField]
    private int cardCountAtStart = 3;

    [SerializeField]
    private int maxCardsInHand = 5;

    [SerializeField]
    private Transform cardListContent;

    [SerializeField]
    private GameObject battleCardPrefab;

    [SerializeField]
    private GameObject spellCardPrefab;

    [SerializeField]
    private GameObject[] piecePrefabs;

    [SerializeField]
    private TextMeshProUGUI cardsLeftTMP;

    [SerializeField]
    private List<BattleCardData> roundCards = new List<BattleCardData>();

    #endregion

    public static BattleCardManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);

        for (int i = 0; i < cardCountAtStart; i++)
        {
            SpawnNewCard();
        }
    }

    /// <summary>
    /// Spawn new card if hand is not full and deck still has cards
    /// </summary>
    public void SpawnNewCard()
    {
        if(cardListContent.childCount >= maxCardsInHand || roundCards.Count == 0)
        {
            return;
        }

        InstantiateCard(Random.Range(0, roundCards.Count));
    }

    /// <summary>
    /// Spawns new card to hand at the beginning of game and each round
    /// </summary>
    /// <param name="index">index of card in player's deck</param>
    private void InstantiateCard(int index)
    {
        BattleCardData newCardData = roundCards[index];
        GameObject newCard = null;

        switch (newCardData.type)
        {
            case eBattleCardType.SPELL:
                newCard = Instantiate(spellCardPrefab, cardListContent);
                break;
            case eBattleCardType.MINION:
                newCard = Instantiate(battleCardPrefab, cardListContent);
                break;
        }

        if (newCard.TryGetComponent(out BattleCardBase script))
        {
            script.data = newCardData;
            script.Initialize();
        }

        newCard.SetActive(true);

        roundCards.RemoveAt(index);

        if(cardsLeftTMP != null)
        {
            cardsLeftTMP.text = roundCards.Count.ToString();
        }
        
    }

    /// <summary>
    /// Get correct minion-prefab by enum
    /// </summary>
    /// <param name="pieceType">type enum</param>
    /// <returns>Chess piece by type</returns>
    public GameObject GetPiecePrefab(ePieceType pieceType)
    {
        return piecePrefabs[(int)pieceType];
    }
}
