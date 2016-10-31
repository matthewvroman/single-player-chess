using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.IO;
using System.Collections;
using System.Collections.Generic;
//http://va.lent.in/unity-make-your-lists-functional-with-reorderablelist/
public class LevelEditorWindow : EditorWindow {

	[MenuItem ("Window/Level Editor")]
	private static void Init () 
	{
        LevelEditorWindow window = (LevelEditorWindow)EditorWindow.GetWindow (typeof (LevelEditorWindow));
        window.Show();
    }

	private List<LevelData>m_levelDataList = null;
	private ReorderableList m_reorderableLevelList = null;
	private LevelData m_currentLevelData = null;
	private LevelData.PlaceableType m_currentPlaceableType;
	private int m_currentTeamIndex = 0;
	private Texture2D m_pawnTexture;
	private Texture2D m_knightTexture;
	private Texture2D m_bishopTexture;
	private Texture2D m_rookTexture;
	private Texture2D m_queenTexture;
	private Texture2D m_kingTexture;


	private void Awake()
	{
		m_pawnTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/GamePieces/chess-pawn.png");
		m_knightTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/GamePieces/chess-knight.png");
		m_bishopTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/GamePieces/chess-bishop.png");
		m_rookTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/GamePieces/chess-rook.png");
		m_queenTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/GamePieces/chess-queen.png");
		m_kingTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/GamePieces/chess-king.png");
	} 

	private void OnEnable()
	{
		
		
		this.titleContent.text = "Level Editor";
		//if(m_levelDataList == null)
		{
			m_levelDataList = new List<LevelData>();
			m_reorderableLevelList = new ReorderableList(m_levelDataList, typeof(LevelData), true, true, true, true);
			m_reorderableLevelList.drawHeaderCallback = DrawLevelListHeaderCallback;
			m_reorderableLevelList.drawElementCallback = DrawLevelListItemCallback;
			m_reorderableLevelList.onSelectCallback = SelectLevelListItemCallback;
			m_reorderableLevelList.onRemoveCallback = RemoveLevelListItemCallback;

			LevelData[] levelAssets = (LevelData[])Resources.LoadAll<LevelData>("Levels/");
			
			int numAssets = levelAssets.Length;
			for(int i=0; i<numAssets; i++)
			{
				LevelData levelData = (LevelData)levelAssets[i];
				if(levelData != null)
				{
					m_levelDataList.Add(levelData);
				}
			}
		}
	}

	public Vector2 sidebarScrollPosition;
	public Vector2 levelAreaScrollPosition;
	private float m_zoomSlider = 1.0f;

	private void OnGUI () {

		if(m_reorderableLevelList==null) return;

		GUILayout.BeginHorizontal();

			sidebarScrollPosition = EditorGUILayout.BeginScrollView(sidebarScrollPosition, false, true, GUILayout.Width (235));
			
			EditorGUILayout.BeginVertical();
				
				if(GUILayout.Button("Save"))
				{
					int numLevels = m_reorderableLevelList.list.Count;
					for(int i=0; i<numLevels; i++)
					{
						LevelData levelData = (LevelData)m_reorderableLevelList.list[i];
						string assetPath = AssetDatabase.GetAssetPath(levelData);
						if(assetPath != "")
						{
							AssetDatabase.RenameAsset(assetPath, i.ToString());
						}
					}

					for(int i=0; i<numLevels; i++)
					{
						string path = "Assets/Resources/Levels/Level" + (i+1).ToString() + ".asset";
						LevelData levelData = (LevelData)m_reorderableLevelList.list[i];
						string assetPath = AssetDatabase.GetAssetPath(levelData);
						if(assetPath == "")
						{
							AssetDatabase.CreateAsset(levelData, path);
						}
						else
						{
							AssetDatabase.RenameAsset(assetPath, "Level" + (i+1).ToString());
						}
					}

					AssetDatabase.SaveAssets();
					AssetDatabase.Refresh();
				}

				m_reorderableLevelList.DoLayoutList();

			EditorGUILayout.EndVertical();
			
			GUILayout.EndScrollView();

			GUILayout.BeginVertical();

				GUILayout.BeginHorizontal();

				if(m_currentLevelData != null)
				{
					
					GUILayout.BeginVertical();
					m_currentPlaceableType = (LevelData.PlaceableType)EditorGUILayout.EnumPopup("Placeable", m_currentPlaceableType);
					m_currentTeamIndex = EditorGUILayout.IntField("Team Index", m_currentTeamIndex);
					m_currentLevelData.m_gridWidth = EditorGUILayout.IntField("Grid Width", m_currentLevelData.m_gridWidth);
					m_currentLevelData.m_gridHeight = EditorGUILayout.IntField("Grid Height", m_currentLevelData.m_gridHeight);
					m_zoomSlider = EditorGUILayout.Slider("Zoom", m_zoomSlider, 0.1f, 1.0f);
					GUILayout.EndVertical();
				}
				
				GUILayout.EndHorizontal();

				levelAreaScrollPosition = EditorGUILayout.BeginScrollView(levelAreaScrollPosition, true, true);
				if(m_currentLevelData != null)
				{
					
					for(int i=0; i<m_currentLevelData.m_gridHeight; i++)
					{
						GUILayout.BeginHorizontal();
						for(int j=0; j<m_currentLevelData.m_gridWidth; j++)
						{
							Texture texture = null;
							GUI.backgroundColor = (i+j)%2==0?Color.grey:Color.white;
							GUI.contentColor = Color.white;
							LevelData.Placement placement = m_currentLevelData.GetPlacement(i, j);
							if(placement != null)
							{
								switch(placement.type)
								{
									case LevelData.PlaceableType.EmptyCell:
										GUI.backgroundColor = Color.clear;
										break;
									case LevelData.PlaceableType.Pawn:
										texture = m_pawnTexture;
										break;
									case LevelData.PlaceableType.Bishop:
										texture = m_bishopTexture;
										break;
									case LevelData.PlaceableType.Knight:
										texture = m_knightTexture;
										break;
									case LevelData.PlaceableType.Rook:
										texture = m_rookTexture;
										break;
									case LevelData.PlaceableType.Queen:
										texture = m_queenTexture;
										break;
									case LevelData.PlaceableType.King:
										texture = m_kingTexture;
										break;
								}
								
								if(placement.team != 0)
								{
									GUI.contentColor = Color.black;
								}
							}
							
							if(GUILayout.Button(texture, new GUILayoutOption[]{GUILayout.Width(50 * m_zoomSlider), GUILayout.Height(50 * m_zoomSlider)}))
							{
								//paint
								switch(m_currentPlaceableType)
								{
									case LevelData.PlaceableType.DefaultCell:
										m_currentLevelData.RemovePlacement(i, j);
										break;
									default:
										m_currentLevelData.SetPlacement(m_currentPlaceableType, i, j, m_currentTeamIndex);
										break;

								}
								
							}
							//GUILayout.Box(texture, new GUILayoutOption[]{GUILayout.Width(50 * m_zoomSlider), GUILayout.Height(50 * m_zoomSlider)});
						}
						GUILayout.EndHorizontal();
					}
				}
				GUILayout.EndScrollView();

			GUILayout.EndVertical();

		GUILayout.EndHorizontal();
    }

	private void DrawLevelListHeaderCallback(Rect rect)
	{
		EditorGUI.LabelField(rect, "Levels");
	}

	private void DrawLevelListItemCallback(Rect rect, int index, bool isActive, bool isFocused)
	{
		EditorGUI.LabelField(rect, "Level " + (index+1));
	}

	private void SelectLevelListItemCallback(ReorderableList list)
	{
		LevelData levelData = m_levelDataList[list.index];
		if(levelData != null)
		{
			m_currentLevelData = levelData;
		}
	}

	private void RemoveLevelListItemCallback(ReorderableList list)
	{

	}
}
