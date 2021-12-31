using UnityEngine;

[System.Serializable]
public class BattleCardData : ScriptableObject
{
    public Sprite cardArt;
    public string cardName;
    [TextArea(1, 5)]
    public string description;
    public int cardNumber;
    [Tooltip("How many actions does playing this reduce from player")]
    public int manaCost = 1;
    [Tooltip("Player must be this level to unlock this spell")]
    public int cardLvl;
    [Tooltip("Can be bought from shop for this price")]
    public int dustCost;
    [Tooltip("How much damage does the minion/spell do?")]
    public int ATK;
    [Tooltip("How much HP does the piece have/Heal ability gives")]
    public int HP;
    [Tooltip("Does card cast a spell or summon minion on board")]
    public eBattleCardType type;
    [Tooltip("Used to instantiate correct prefab")]
    public ePieceType pieceToSummon;
    [Tooltip("How many bottom rows are included in summon-zone?")]
    public int summonRows = 2;
    public ePieceTeam affectedTeam = ePieceTeam.Enemy;
    public eSpellType spellType = eSpellType.NONE;
    public int duration = 0;
}
