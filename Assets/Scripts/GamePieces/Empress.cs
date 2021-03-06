﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Empress : GamePiece {

	public override List<GridCell>GetMovementCells(GameGrid grid, GridCell cell)
	{
		List<GridCell>cells = grid.GetLeapCells(cell, 2, 1);
		cells.AddRange(grid.GetParallelCells(cell));

		return cells;
	}

	public override List<GridCell>GetAttackCells(GameGrid grid, GridCell cell)
	{
		return GetMovementCells(grid, cell);
	}
}
