using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class King : GamePiece {

	public override List<GridCell>GetMovementCells(GameGrid grid, GridCell cell)
	{
		List<GridCell>cells = grid.GetDiagonalCells(cell, 1);
		cells.AddRange(grid.GetParallelCells(cell, 1));
		return cells;
	}

	public override List<GridCell>GetAttackCells(GameGrid grid, GridCell cell)
	{
		return GetMovementCells(grid, cell);
	}
}
