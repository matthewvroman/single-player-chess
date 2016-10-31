using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class LevelData : ScriptableObject{
	
	public enum PlaceableType
	{
		EmptyCell,
		DefaultCell,
		Pawn,
		Knight,
		Bishop,
		Rook,
		Queen,
		King
	}

	public int m_gridWidth=8;
	public int m_gridHeight=8;

	[System.Serializable]
	public class Placement
	{
		public int x;
		public int y;
		public PlaceableType type;
		public int team;

		public Placement(PlaceableType placeableType, int cellX, int cellY, int teamIndex=0)
		{
			type = placeableType;
			x = cellX;
			y = cellY;
			team = teamIndex;
		}
	}

	public List<Placement>m_placements = new List<Placement>();

	public Placement GetPlacement(int x, int y)
	{
		int numPlacements = m_placements.Count;
		for(int i=0; i<numPlacements; i++)
		{
			Placement placement = m_placements[i];
			if(placement.x == x && placement.y == y)
			{
				return placement;
			}
		}

		return null;
	}

	public void SetPlacement(PlaceableType type, int x, int y, int team=0)
	{
		Placement currentPlacement = GetPlacement(x, y);
		if(currentPlacement != null)
		{
			currentPlacement.type = type;
			currentPlacement.team = team;
		}
		else
		{
			m_placements.Add(new Placement(type, x, y, team));
		}
	}

	public void RemovePlacement(int x, int y)
	{
		Placement currentPlacement = GetPlacement(x, y);
		if(currentPlacement != null)
		{
			m_placements.Remove(currentPlacement);
		}
	}
}
