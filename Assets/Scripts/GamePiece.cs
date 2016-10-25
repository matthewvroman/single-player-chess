using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GamePiece : MonoBehaviour {

	private int m_teamId;
	public int TeamId
	{
		get
		{
			return m_teamId;
		}
	}

	public virtual void SetTeamId(int teamId)
	{
		m_teamId = teamId;

		SpriteRenderer spriteRenderer = this.GetComponent<SpriteRenderer>();
		if(spriteRenderer != null)
		{
			spriteRenderer.color = TeamManager.Instance.GetTeamColor(teamId);
		}
	}

	public virtual List<GridCell>GetMovementCells(GameGrid grid, GridCell cell)
	{
		return null;
	}

	public virtual List<GridCell>GetAttackCells(GameGrid grid, GridCell cell)
	{
		return null;
	}
}
