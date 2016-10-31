using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Wazir : GamePiece {

	public override List<GridCell>GetMovementCells(GameGrid grid, GridCell cell)
	{
		return grid.GetParallelCells(cell, 1);
	}

	public override List<GridCell>GetAttackCells(GameGrid grid, GridCell cell)
	{
		return GetMovementCells(grid, cell);
	}
}
