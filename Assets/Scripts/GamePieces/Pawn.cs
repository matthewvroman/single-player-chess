using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pawn : GamePiece {

	private bool m_hasMoved = false;
	private int Direction
	{
		get
		{
			return TeamId==TeamManager.Instance.PlayerTeamId?1:-1;
		}
	}

	public override List<GridCell>GetMovementCells(GameGrid grid, GridCell cell)
	{
		List<GridCell> cells = GetAttackCells(grid, cell);
		cells.AddRange(GetMovementOnlyCells(grid, cell));
		return cells;
	}

	private List<GridCell> GetMovementOnlyCells(GameGrid grid, GridCell cell)
	{
		List<GridCell>cells = grid.GetCellsForDirection(cell, 0, Direction, m_hasMoved?1:2);
		int numCells = cells.Count;
		List<GridCell>viableCells = new List<GridCell>();
		for(int i=0; i<numCells; i++)
		{
			GridCell potentialCell = cells[i];
			if(potentialCell.GamePiece == null)
			{
				viableCells.Add(potentialCell);
			}
		}
		return viableCells;
	}

	public override List<GridCell>GetAttackCells(GameGrid grid, GridCell cell)
	{
		List<GridCell>cells = grid.GetCellsForDirection(cell, 1, Direction, 1);
		cells.AddRange(grid.GetCellsForDirection(cell, -1, Direction, 1));
		
		int numCells = cells.Count;
		List<GridCell>viableCells = new List<GridCell>();
		for(int i=0; i<numCells; i++)
		{
			GridCell potentialCell = cells[i];
			if(potentialCell.GamePiece != null && potentialCell.GamePiece.TeamId != TeamId)
			{
				viableCells.Add(potentialCell);
			}
		}

		return viableCells;
	}

	private void OnEnable()
	{
		GameGrid.OnGamePieceMoved += HandleOnGamePieceMoved;
	}

	private void OnDisable()
	{
		GameGrid.OnGamePieceMoved -= HandleOnGamePieceMoved;
	}

	private void HandleOnGamePieceMoved(GamePiece gamePiece)
	{
		if(gamePiece == this)
		{
			m_hasMoved = true;
		}
	}
}
