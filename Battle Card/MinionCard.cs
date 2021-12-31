using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Battle;

public class MinionCard : BattleCardBase
{	
    private List<CellData> zone = new List<CellData>();

    /// <summary>
    /// Check is there is space on Game Board to summon minion
    /// </summary>
    /// <returns>true if there is empty space</returns>
    protected override bool IsPlayable()
    {
        DefineArea(data.summonRows);

        if(zone.Count > 0)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Get free cells from Game Board
    /// </summary>
    /// <param name="rows">Number of bottom rows to check</param>
    private void DefineArea(int rows)
    {
        if(rows > Board.Rows)
        {
            rows = Board.Rows;
        }

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < Board.Columns; c++)
            {
                CellData cell = Board.GetCellData(new Vector2Int(r, c));

                //Cell is free, can summon here
                if (cell != null && cell.CurrentPiece == null)
                {
                    zone.Add(cell);
                }
            }
        }
    }

    /// <summary>
    /// Card was played, send info to InputHandler
    /// </summary>
    protected override void Cast()
    { 
        PlayerInputHandler.SummonPieceInfo(CardManager.GetPiecePrefab(data.pieceToSummon), zone);
        Destroy(dummy);
        Destroy(gameObject);
    }
}
