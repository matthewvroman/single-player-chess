using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Queen : GamePiece {

	public override List<GridCell>GetMovementCells(GameGrid grid, GridCell cell)
	{
		List<GridCell>cells = grid.GetDiagonalCells(cell);
		cells.AddRange(grid.GetParallelCells(cell));
		return cells;
	}

	public override List<GridCell>GetAttackCells(GameGrid grid, GridCell cell)
	{
		return GetMovementCells(grid, cell);
	}
}
