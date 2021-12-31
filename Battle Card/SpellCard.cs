using System.Collections.Generic;
using UnityEngine;
using static Battle;

public class SpellCard : BattleCardBase
{
    private List<CellData> cellsToCast = new List<CellData>();

    /// <summary>
    /// Finds friendly or enemy minions on board to which spell can be cast.
    /// </summary>
    /// <returns>true if found target minions</returns>
    protected override bool IsPlayable()
    {
        FindMinions(data.affectedTeam);
        if(cellsToCast.Count > 0)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Card was played, send info to input handler
    /// </summary>
    protected override void Cast()
    {
        PlayerInputHandler.SpellInfo(data, cellsToCast);
        Destroy(dummy);
        Destroy(gameObject);
    }


    protected void FindMinions(ePieceTeam team)
    {
        for (int r = 0; r < Board.Rows; r++)
        {
            for (int c = 0; c < Board.Columns; c++)
            {
                CellData cell = Board.GetCellData(new Vector2Int(r, c));

                if (cell.CurrentPiece != null && cell.CurrentPiece.ePieceTeam == team)
                {
                    cellsToCast.Add(cell);
                }
            }
        }
    }
}
