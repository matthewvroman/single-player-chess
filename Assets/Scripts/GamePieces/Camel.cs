using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Camel : GamePiece {

	public override List<GridCell>GetMovementCells(GameGrid grid, GridCell cell)
	{
		List<GridCell>cells = grid.GetCellsForDirection(cell, 3, 1, 1);
		cells.AddRange(grid.GetCellsForDirection(cell, 3, -1, 1));
		cells.AddRange(grid.GetCellsForDirection(cell, -3, -1, 1));
		cells.AddRange(grid.GetCellsForDirection(cell, -3, 1, 1));
		cells.AddRange(grid.GetCellsForDirection(cell, 1, 3, 1));
		cells.AddRange(grid.GetCellsForDirection(cell, -1, 3, 1));
		cells.AddRange(grid.GetCellsForDirection(cell, -1, -3, 1));
		cells.AddRange(grid.GetCellsForDirection(cell, 1, -3, 1));
		return cells;
	}

	public override List<GridCell>GetAttackCells(GameGrid grid, GridCell cell)
	{
		return GetMovementCells(grid, cell);
	}
}
