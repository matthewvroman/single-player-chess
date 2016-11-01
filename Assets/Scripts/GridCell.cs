using UnityEngine;
using System;
using System.Collections;

public class GridCell : MonoBehaviour{

	public static Action<GridCell>OnClicked;
	public static Action<GridCell, GamePiece>OnGamePieceSet;

	private int m_x;
	public int X
	{
		get
		{
			return m_x;
		}
	}

	private int m_y;
	public int Y
	{
		get
		{
			return m_y;
		}
	}

	private GamePiece m_gamePiece;
	public GamePiece GamePiece
	{
		get
		{
			return m_gamePiece;
		}
	}

	private SpriteRenderer m_spriteRenderer;

	public void SetGamePiece(GamePiece gamePiece)
	{
		if(gamePiece != null)
		{
			if(m_gamePiece != null)
			{
				//TEMP
				Debug.Log("Taking piece");
				GameObject.Destroy(m_gamePiece.gameObject);
				m_gamePiece = null;
			}

			gamePiece.transform.SetParent(this.transform);
			gamePiece.transform.localPosition = Vector3.back;
		}

		m_gamePiece = gamePiece;

		if(OnGamePieceSet != null) OnGamePieceSet(this, m_gamePiece);
	}

	public void SetCoords(int x, int y)
	{
		m_x = x;
		m_y = y;
	}

	public void Awake()
	{
		this.gameObject.AddComponent<BoxCollider2D>();

		m_spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
	}
	
	//must be attached to a spriterenderer
	private void OnMouseDown()
	{
		if(OnClicked != null)
		{
			OnClicked(this);
		}
	}

	public void Highlight()
	{
		Color color = m_spriteRenderer.color;
		color.a = 0.25f;
		m_spriteRenderer.color = color;
	}

	public void Dehighlight()
	{
		Color color = m_spriteRenderer.color;
		color.a = 1.0f;
		m_spriteRenderer.color = color;
	}

}
