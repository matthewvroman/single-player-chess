using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TeamManager {

	private static TeamManager s_instance;
	public static TeamManager Instance
	{
		get
		{
			if(s_instance == null)
			{
				s_instance = new TeamManager();
			}
			return s_instance;
		}
	}

	private Dictionary<int, Color>m_teamColors;

	private int m_playerTeamId = 0;
	public int PlayerTeamId
	{
		get
		{
			return m_playerTeamId;
		}
	}

	public TeamManager()
	{
		m_teamColors = new Dictionary<int, Color>();
	}

	public void SetPlayerTeamId(int teamId)
	{
		m_playerTeamId = teamId;
	}

	public void SetTeamColor(int teamId, Color color)
	{
		m_teamColors[teamId] = color;
	}

	public Color GetTeamColor(int teamId)
	{
		return m_teamColors[teamId];
	}
}
