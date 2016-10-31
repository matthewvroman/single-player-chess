using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Camel : GamePiece {

	public override List<GridCell>GetMovementCells(GameGrid grid, GridCell cell)
	{
		return grid.GetLeapCells(cell, 3, 1);
	}

	public override List<GridCell>GetAttackCells(GameGrid grid, GridCell cell)
	{
		return GetMovementCells(grid, cell);
	}
}
