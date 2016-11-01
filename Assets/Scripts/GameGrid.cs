using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameGrid : MonoBehaviour {

	public static Action<GamePiece>OnGamePieceMoved;

	[SerializeField]
	private int m_gridWidth = 8;
	[SerializeField]
	private int m_gridHeight = 8;
	[SerializeField]
	private float m_cellWidth = 5.12f;
	[SerializeField]
	private float m_cellHeight = 5.12f;

	[SerializeField]
	private GameObject m_evenCellBackground;
	[SerializeField]
	private GameObject m_oddCellBackground;

	private List<List<GridCell>>m_cells;

	private List<GridCell> m_selectedCells;
	private GridCell m_clickedCell;

	private void Awake()
	{
		
		TeamManager.Instance.SetTeamColor(0, Color.white);
		TeamManager.Instance.SetTeamColor(1, Color.black);

		//TEMP
		LoadLevel(2);
	}

	public void LoadLevel(int levelNumber)
	{
		LevelData levelData = Resources.Load<LevelData>("Levels/Level" + levelNumber.ToString());
		LoadLevel(levelData);
	}

	public void LoadLevel(LevelData levelData)
	{
		m_gridHeight = levelData.m_gridHeight;
		m_gridWidth = levelData.m_gridWidth;
		CreateGrid(levelData);
	}

	private void OnEnable()
	{
		GridCell.OnClicked += HandleOnGridCellClicked;
	}

	private void OnDisable()
	{
		GridCell.OnClicked -= HandleOnGridCellClicked;
	}

	private void CreateGrid(LevelData levelData)
	{
		m_cells = new List<List<GridCell>>();
		for(int x=0; x<m_gridWidth; x++)
		{
			for(int y=0; y<m_gridHeight; y++)
			{
				GameObject prefab = null;
				if((x+y)%2==0)
				{
					prefab = m_evenCellBackground;
				}
				else
				{
					prefab = m_oddCellBackground;
				}

				LevelData.Placement placement = null;
				if(levelData != null)
				{
					placement = levelData.GetPlacement(x, y);
				}

				GameObject gameObject = (GameObject)GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity);
				gameObject.transform.SetParent(this.transform);
				gameObject.transform.localPosition = new Vector3((x - m_gridWidth/2.0f)*m_cellWidth + m_cellWidth/2.0f, -(y - m_gridHeight/2.0f)*m_cellHeight - m_cellWidth/2.0f);

				if(y==0)
				{
					m_cells.Add(new List<GridCell>());
				}

				if(placement != null && placement.type == LevelData.PlaceableType.EmptyCell)
				{
					Destroy(gameObject);
					m_cells[m_cells.Count-1].Add(null);
				}
				else
				{
					GridCell gridCell = gameObject.AddComponent<GridCell>();
					gridCell.SetCoords(x, y);
					m_cells[m_cells.Count-1].Add(gridCell);
				}

				if(placement != null)
				{
					CreateGamePiece(placement.type, placement.x, placement.y, placement.team);
				}
			}
		}
	}

	public void CreateGamePiece(LevelData.PlaceableType type, int x, int y, int teamId)
	{
		Debug.Log(type.ToString() + ", " + x + ", " + y + ", " + teamId);
		switch(type)
		{
			case LevelData.PlaceableType.Bishop:
			case LevelData.PlaceableType.King:
			case LevelData.PlaceableType.Knight:
			case LevelData.PlaceableType.Pawn:
			case LevelData.PlaceableType.Queen:
			case LevelData.PlaceableType.Rook:
				CreateGamePiece("GamePiece" + type.ToString(), x, y, teamId);
				break;
		}
	}

	public void CreateGamePiece(string pieceName, int x, int y, int teamId)
	{
		GameObject resourcePrefab = Resources.Load<GameObject>(pieceName);
		GameObject gameObject = (GameObject)GameObject.Instantiate(resourcePrefab, Vector3.zero, Quaternion.identity);
		GamePiece gamePiece = gameObject.GetComponent<GamePiece>();
		if(gamePiece == null)
		{
			Debug.LogError("Tried to create a game piece from resource " + pieceName + " but it didn't have a GamePiece component attached to it.");
			return;
		}
		gamePiece.SetTeamId(teamId);
		AddGamePiece(gamePiece, x, y);

	}

	public void AddGamePiece(GamePiece gamePiece, int x, int y)
	{
		GridCell cell = GetCell(x, y);
		if(cell != null)
		{
			AddGamePiece(gamePiece, cell);
		}

	}

	public void AddGamePiece(GamePiece gamePiece, GridCell gridCell)
	{
		gridCell.SetGamePiece(gamePiece);
	}

	public void MoveGamePiece(GamePiece gamePiece, GridCell fromCell, GridCell toCell)
	{
		if(gamePiece == fromCell.GamePiece)
		{
			fromCell.SetGamePiece(null);
		}
		AddGamePiece(gamePiece, toCell);

		if(OnGamePieceMoved != null)
		{
			OnGamePieceMoved(gamePiece);
		}
	}

	public GridCell GetCell(int x, int y)
	{
		if(x<0 || x>=m_gridWidth) return null;
		if(y<0 || y>=m_gridHeight) return null;

		return m_cells[x][y];
	}

	private void HandleOnGridCellClicked(GridCell gridCell)
	{
		if(m_selectedCells != null)
		{
			int numSelectedCells = m_selectedCells.Count;
			for(int i=0; i<numSelectedCells; i++)
			{
				m_selectedCells[i].Dehighlight();
			}
		}

		if(m_clickedCell != null && gridCell==m_clickedCell)
		{
			m_clickedCell = null;
			return;
		}

		m_selectedCells = new List<GridCell>();
		m_selectedCells.Add(gridCell);
		
		if(gridCell.GamePiece != null 
		&& gridCell.GamePiece.TeamId == TeamManager.Instance.PlayerTeamId
		&& (!m_clickedCell || m_clickedCell.GamePiece.TeamId == gridCell.GamePiece.TeamId))
		{
			m_selectedCells.AddRange(gridCell.GamePiece.GetMovementCells(this, gridCell));
		}
		else if(m_clickedCell && m_clickedCell.GamePiece != null
		&& m_clickedCell.GamePiece.GetMovementCells(this, m_clickedCell).IndexOf(gridCell) != -1)
		{
			MoveGamePiece(m_clickedCell.GamePiece, m_clickedCell, gridCell);
			m_clickedCell = null;
			return;
		}
		else
		{
			//Clicked on empty cell
			m_clickedCell = null;
			return;
		}

		for(int i=0; i<m_selectedCells.Count; i++)
		{
			m_selectedCells[i].Highlight();
		}

		m_clickedCell = gridCell;
	}

	public List<GridCell>GetDiagonalCells(GridCell gridCell, int maxIterations=-1)
	{
		List<GridCell>cells = GetCellsForDirection(gridCell, 1, 1, maxIterations);
		cells.AddRange(GetCellsForDirection(gridCell, 1, -1, maxIterations));
		cells.AddRange(GetCellsForDirection(gridCell, -1, 1, maxIterations));
		cells.AddRange(GetCellsForDirection(gridCell, -1, -1, maxIterations));
		return cells;
	}

	public List<GridCell>GetParallelCells(GridCell gridCell, int maxIterations=-1)
	{
		List<GridCell>cells = GetCellsForDirection(gridCell, 1, 0, maxIterations);
		cells.AddRange(GetCellsForDirection(gridCell, -1, 0, maxIterations));
		cells.AddRange(GetCellsForDirection(gridCell, 0, 1, maxIterations));
		cells.AddRange(GetCellsForDirection(gridCell, 0, -1, maxIterations));
		return cells;
	}

	public List<GridCell>GetLeapCells(GridCell gridCell, int leapM, int leapN)
	{
		List<GridCell>cells = GetCellsForDirection(gridCell, leapM, leapN, 1);
		cells.AddRange(GetCellsForDirection(gridCell, leapM, -leapN, 1));
		cells.AddRange(GetCellsForDirection(gridCell, -leapM, -leapN, 1));
		cells.AddRange(GetCellsForDirection(gridCell, -leapM, leapN, 1));
		cells.AddRange(GetCellsForDirection(gridCell, leapN, leapM, 1));
		cells.AddRange(GetCellsForDirection(gridCell, -leapN, leapM, 1));
		cells.AddRange(GetCellsForDirection(gridCell, -leapN, -leapM, 1));
		cells.AddRange(GetCellsForDirection(gridCell, leapN, -leapM, 1));
		return cells;
	}

	public List<GridCell>GetCellsForDirection(GridCell gridCell, int dirX, int dirY, int maxIterations=-1, bool travelThroughGamePieces=false)
	{
		List<GridCell>cells = new List<GridCell>();

		int searchX = gridCell.X;
		int searchY = gridCell.Y;
		int endX = dirX > 0 ? m_gridWidth - 1 : 0;
		int endY = dirY > 0 ? m_gridHeight - 1 : 0;
		int iteration = 0;
		bool hitGamePiece = false;

		while(((((searchX < endX && dirX > 0) || (searchX > 0 && dirX < 0)) && (dirX != 0))
		|| (((searchY < endY && dirY > 0) || (searchY > 0 && dirY < 0)) && (dirY != 0)))
		&& (iteration < maxIterations || maxIterations < 0)
		&& (!hitGamePiece && !travelThroughGamePieces || travelThroughGamePieces))
		{
			iteration++;
			searchX+=dirX;
			searchY+=dirY;
			GridCell cell = GetCell(searchX, searchY);
			if(cell != null)
			{
				if(cell.GamePiece != null)
				{
					hitGamePiece = true;
					if(gridCell.GamePiece && gridCell.GamePiece.TeamId == cell.GamePiece.TeamId)
					{
						continue;
					}
				}
				cells.Add(cell);
			}
		}

		return cells;
	}
}
