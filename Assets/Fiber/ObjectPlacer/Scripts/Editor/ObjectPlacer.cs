using System.Collections.Generic;
using System.Linq;
using PathCreation;
using PathCreation.Examples;
using UnityEditor;
using UnityEngine;

namespace Fiber.ObjectPlacer
{
	public class ObjectPlacer : EditorWindow
	{
		[MenuItem("Fiber/Runner/", false, 1000)]
		[MenuItem("Fiber/Runner/Object Placer", false, 1000)]
		public static void OpenWindow()
		{
			//Type type = Type.GetType("UnityEditor.InspectorWindow, UnityEditor.dll");
			_window = GetWindow<ObjectPlacer>("Object Placer");
		}

		int fiberHash = "fiberTools".GetHashCode();
		int controlID;
		public string groupName;
		public bool isPlacerActive;
		public bool placeSingle;
		public int fileName = 0;
		Vector2 scrollPos;
		private static ObjectPlacer _window = null;

		public List<ObjectGroup> objectGroups = new List<ObjectGroup>();

		public SerializedObject so;
		public SerializedProperty propGroupName;

		public List<GameObject> spawnedObjects = new List<GameObject>();

		private bool pathDetected
		{
			get
			{
				var path = FindObjectOfType<PathCreator>();
				return path != null;
			}
		}

		private void OnEnable()
		{
			_window = this;
			SceneView.duringSceneGui += DuringSceneGUI;
			objectGroups.Clear();
			so = new SerializedObject(this);
			propGroupName = so.FindProperty("groupName");
			GetExistingObjectGroups();
			fileName = objectGroups.Count;
		}

		private void OnDisable()
		{
			SceneView.duringSceneGui -= DuringSceneGUI;
		}

		public void GetExistingObjectGroups()
		{
			if (AssetDatabase.IsValidFolder("Assets/Fiber/ObjectPlacer/Presets"))
			{
				string[] guids = AssetDatabase.FindAssets("t:ObjectGroup", new[] { "Assets/Fiber/ObjectPlacer/Presets" });
				IEnumerable<string> paths = guids.Select(AssetDatabase.GUIDToAssetPath);
				var groups = paths.Select(AssetDatabase.LoadAssetAtPath<ObjectGroup>).ToArray();
				foreach (var group in groups)
				{
					group.Init(group.groupName);
					objectGroups.Add(group);
				}
			}
		}

		private void OnGUI()
		{
			_window.minSize = new Vector2(600, 200);
			_window.maxSize = new Vector2(600, 2000);

			so.Update();

			if (!pathDetected && !placeSingle)
			{
				placeSingle = true;
			}

			EditorGUILayout.BeginVertical();
			scrollPos = GUILayout.BeginScrollView(scrollPos, false, false);
			EditorGUILayout.Space(10);

			EditorGUILayout.BeginHorizontal("helpBox");
			EditorGUILayout.PropertyField(propGroupName, GUILayout.Width(300));
			if (GUILayout.Button("Add Group", GUILayout.Width(100)))
			{
				if (string.IsNullOrEmpty(groupName))
				{
					groupName = fileName.ToString();
					fileName++;
				}

				AddGroup(groupName);
				groupName = "";
			}

			EditorGUILayout.EndHorizontal();

			if (objectGroups.Count > 0)
			{
				CheckForChange();
				CreateListPropertyFields();
				CreatePlacementMap();
				CreateToggles();
				CreateButtons();
				CreatePlacerActiveToggle();
			}

			EditorGUILayout.EndScrollView();
			EditorGUILayout.EndVertical();
			so.ApplyModifiedProperties();
		}

		#region OnGUIMethods

		private void AddGroup(string name)
		{
			var newGroup = CreateInstance<ObjectGroup>();
			objectGroups.Add(newGroup);
			newGroup.Init(name);
			if (AssetDatabase.IsValidFolder("Assets/Fiber/ObjectPlacer/Presets"))
			{
				string path = $"Assets/Fiber/ObjectPlacer/Presets/{newGroup.groupName}.asset";
				AssetDatabase.CreateAsset(newGroup, path);
			}
			else
			{
				AssetDatabase.CreateFolder("Assets/Fiber/ObjectPlacer", "Presets");
				string path = $"Assets/Fiber/ObjectPlacer/Presets/{newGroup.groupName}.asset";
				AssetDatabase.CreateAsset(newGroup, path);
			}
		}

		private void RemoveGroup(ObjectGroup group)
		{
			objectGroups.Remove(group);
			string path = $"Assets/Fiber/ObjectPlacer/Presets/{group.groupName}.asset";
			AssetDatabase.DeleteAsset(path);
			Repaint();
		}

		private void CheckForChange()
		{
			foreach (var group in objectGroups)
			{
				if (group != null)
				{
					if (group.CheckChange())
					{
						Repaint();
					}
				}
				else
				{
					objectGroups.Remove(group);
					CheckForChange();
					break;
				}
			}
		}

		private void CreateListPropertyFields()
		{
			foreach (var group in objectGroups)
			{
				group.so.Update();
				EditorGUILayout.BeginHorizontal("helpBox");
				EditorGUILayout.BeginVertical();
				EditorGUILayout.LabelField(group.groupName, GUILayout.Width(200));
				EditorGUILayout.PropertyField(group.propObjectList, GUILayout.Width(300));
				EditorGUILayout.EndVertical();
				if (GUILayout.Button("Remove Group", GUILayout.Width(200)))
				{
					RemoveGroup(group);
					break;
				}

				EditorGUILayout.EndHorizontal();
				group.so.ApplyModifiedProperties();
			}

			EditorGUILayout.Space(10);
		}

		private void CreatePlacementMap()
		{
			if (!placeSingle && isPlacerActive && pathDetected)
			{
				EditorGUILayout.BeginVertical("helpBox");
				EditorGUILayout.LabelField("Placement Map", EditorStyles.boldLabel);
				var labelWidth = EditorGUIUtility.labelWidth;
				var fieldWidth = EditorGUIUtility.fieldWidth;
				EditorGUIUtility.labelWidth = 200;
				EditorGUILayout.Space(5);
				foreach (var group in objectGroups)
				{
					group.so.Update();
					EditorGUILayout.BeginVertical("helpBox");
					EditorGUILayout.LabelField(group.groupName + " Placement", EditorStyles.boldLabel);
					EditorGUILayout.PropertyField(group.propInitialOffset, new GUIContent("Initial Offset", "Yerleştirilecek İlk objenin başlangıç noktasına uzaklığı."), GUILayout.Width(250));
					EditorGUILayout.PropertyField(group.propPlacementInterval, new GUIContent("Placement Interval", "İlk objeden sonra kaç birimde bir alt grup oluşturulacağı."),
						GUILayout.Width(250));
					group.propPlacementInterval.intValue = Mathf.Max(1, group.propPlacementInterval.intValue);
					EditorGUILayout.PropertyField(group.propElementPlacementInterval, new GUIContent("Element Placement Interval", "Alt grup içindeki objelerin birbirlerinden uzaklıkları."),
						GUILayout.Width(250));
					group.propPlacementInterval.intValue = Mathf.Max(1, group.propPlacementInterval.intValue);
					EditorGUILayout.PropertyField(group.propGroupCountMinMax,
						new GUIContent("Sub Group Count(Min-Max)", "Alt grupların içereceği en az ve en çok obje adedi. Obje adedi verilen değerler arasında rastgele belirlenir."),
						GUILayout.Width(250));
					EditorGUILayout.PropertyField(group.propUseRandomX,
						new GUIContent("Use Random X", "Seçili değilse alt gruplar tam path üstüne yerleştirilir, seçiliyse path üstünde rastgele sağa/sola dağıtılır."), GUILayout.Width(250));
					EditorGUILayout.Space(5);
					EditorGUILayout.EndVertical();
					group.so.ApplyModifiedProperties();
				}

				EditorGUIUtility.labelWidth = labelWidth;
				EditorGUILayout.EndVertical();
				EditorGUILayout.Space(10);
			}
		}

		private void CreatePlacerActiveToggle()
		{
			EditorGUILayout.Space(20);
			Color c = GUI.backgroundColor;
			GUI.backgroundColor = isPlacerActive ? Color.green : Color.red;
			isPlacerActive = GUILayout.Toggle(isPlacerActive, isPlacerActive ? "Placer is Active" : "Placer is InActive", "button", GUILayout.Height(30));
			GUI.backgroundColor = c;
			EditorGUILayout.Space(20);
		}

		private void CreateToggles()
		{
			foreach (var group in objectGroups)
			{
				group.so.Update();
				EditorGUILayout.Space(20);
				EditorGUILayout.BeginVertical("helpBox");
				EditorGUILayout.LabelField(group.groupName);
				foreach (var toggle in group.toggleList)
				{
					EditorGUILayout.BeginHorizontal();
					Texture icon = AssetPreview.GetAssetPreview(toggle.prefab);
					var content = new GUIContent(toggle.prefab.name, icon);

					if (toggle.isActive && placeSingle)
					{
						ClearToggles();
						toggle.isActive = true;
					}

					toggle.isActive = GUILayout.Toggle(toggle.isActive, content, GUILayout.Height(20), GUILayout.Width(100));
					EditorGUILayout.Space(20);
					toggle.offset = EditorGUILayout.Vector3Field("Offset", toggle.offset, GUILayout.Width(300));
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.Space(10);
				}

				group.so.ApplyModifiedProperties();
				EditorGUILayout.EndVertical();
			}
		}

		private void CreateButtons()
		{
			EditorGUILayout.Space(30);
			EditorGUILayout.BeginHorizontal("helpBox");
			if (pathDetected)
				placeSingle = GUILayout.Toggle(placeSingle, "Place Single Item");

			//Button to make all toggles false and clear all offsets
			EditorGUILayout.BeginVertical();
			if (!placeSingle && pathDetected)
			{
				if (GUILayout.Button("Select All", GUILayout.Width(200)))
				{
					SelectAll();
				}
			}

			if (GUILayout.Button(pathDetected ? "Clear Toggles" : "Clear Toggle", GUILayout.Width(200)))
			{
				ClearToggles();
			}

			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();
		}

		private void ClearToggles()
		{
			foreach (var toggle in GetAllToggles())
			{
				toggle.isActive = false;
			}
		}

		private void SelectAll()
		{
			if (placeSingle) return;
			foreach (var toggle in GetAllToggles())
			{
				toggle.isActive = true;
			}
		}

		private List<ToggleObject> GetAllToggles()
		{
			var toggles = new List<ToggleObject>();
			foreach (var group in objectGroups)
			{
				toggles.AddRange(group.toggleList);
			}

			return toggles;
		}

		#endregion

		private void DuringSceneGUI(SceneView sceneView)
		{
			if (!isPlacerActive) return;
			Event e = Event.current;
			controlID = GUIUtility.GetControlID(fiberHash, FocusType.Passive);

			if (e.type == EventType.Layout)
			{
				HandleUtility.AddDefaultControl(controlID);
				return;
			}

			if (e.type == EventType.MouseUp && e.button == 0)
			{
				PlaceObjects(e);
				e.Use();
			}

			HandleUtility.Repaint();
		}

		private void PlaceObjects(Event e)
		{
			if (!isPlacerActive) return;
			if (placeSingle)
			{
				var selection = GetActiveSelection();
				if (selection != null)
				{
					RaycastHit hit;
					Ray ray = HandleUtility.GUIPointToWorldRay(new Vector2(e.mousePosition.x, e.mousePosition.y));

					if (Physics.Raycast(ray, out hit))
					{
						GameObject spawn = PrefabUtility.InstantiatePrefab(selection.prefab) as GameObject;

						if (hit.transform.parent != null && hit.transform.parent.TryGetComponent(out PathCreator path))
						{
							var time = path.path.GetClosestTimeOnPath(hit.point);
							spawn.transform.rotation = path.path.GetRotation(time, EndOfPathInstruction.Stop);
							spawn.transform.position = path.path.GetPointAtTime(time, EndOfPathInstruction.Stop) + spawn.transform.right * selection.offset.x +
							                           spawn.transform.up * selection.offset.y + spawn.transform.forward * selection.offset.z;
							spawn.transform.eulerAngles = new Vector3(spawn.transform.eulerAngles.x, spawn.transform.eulerAngles.y, 0f);
						}
						else
						{
							spawn.transform.up = hit.normal;
							spawn.transform.position = hit.point;
							spawn.transform.position += spawn.transform.right * selection.offset.x + spawn.transform.up * selection.offset.y + spawn.transform.forward * selection.offset.z;
						}

						Undo.RegisterCreatedObjectUndo(spawn, "undo spawn");
					}
				}
				else
				{
					Debug.LogWarning("No objects selected!");
				}
			}
			else
			{
				if (!ValidateOperation())
				{
					Debug.LogWarning("No Objects Selected!");
				}

				RaycastHit hit;
				Ray ray = HandleUtility.GUIPointToWorldRay(new Vector2(e.mousePosition.x, e.mousePosition.y));

				if (Physics.Raycast(ray, out hit))
				{
					if (hit.transform.parent != null && hit.transform.parent.TryGetComponent(out PathCreator path))
					{
						if (spawnedObjects.Count > 0)
						{
							foreach (var item in spawnedObjects)
							{
								DestroyImmediate(item);
							}

							spawnedObjects.Clear();
						}

						Undo.IncrementCurrentGroup();
						Undo.SetCurrentGroupName("PlaceObjects");
						var undoGroupIndex = Undo.GetCurrentGroup();

						foreach (var group in objectGroups)
						{
							if (ValidateList(group.toggleList))
							{
								var amountToPlace = Mathf.FloorToInt((path.path.length - group.initialOffset) / group.placementInterval);
								if (amountToPlace == 0)
								{
									Debug.Log("Can't place objects because path lenght and placement distance don't match. Either path is too short or placement is too long");
									return;
								}

								GameObject parent = new GameObject(group.groupName);
								Undo.RegisterCreatedObjectUndo(parent, "");
								parent.transform.SetParent(path.transform);
								spawnedObjects.Add(parent);

								for (int i = 0; i < amountToPlace; i++)
								{
									var randCount = UnityEngine.Random.Range(group.subGroupCountMinMax.x, group.subGroupCountMinMax.y);
									var randXOffset = UnityEngine.Random.Range(-path.GetComponent<RoadMeshCreator>().roadWidth + 1, path.GetComponent<RoadMeshCreator>().roadWidth - 1);
									for (int j = 0; j < randCount; j++)
									{
										var randToggle = GetRandom(group.toggleList);
										var spawn = (GameObject)PrefabUtility.InstantiatePrefab(randToggle.prefab, parent.transform);
										spawnedObjects.Add(spawn);
										var distance = group.initialOffset + j * group.elementPlacementInterval + i * group.placementInterval;
										spawn.transform.rotation = path.path.GetRotationAtDistance(distance, EndOfPathInstruction.Stop);
										spawn.transform.position = group.useRandomX
											? path.path.GetPointAtDistance(distance, EndOfPathInstruction.Stop) + spawn.transform.right * randXOffset + spawn.transform.up * randToggle.offset.y
											: path.path.GetPointAtDistance(distance, EndOfPathInstruction.Stop) + spawn.transform.up * randToggle.offset.y;
										spawn.transform.eulerAngles = new Vector3(spawn.transform.eulerAngles.x, spawn.transform.eulerAngles.y, 0f);
										Undo.RegisterCreatedObjectUndo(spawn, "");
									}
								}
							}
						}

						Undo.CollapseUndoOperations(undoGroupIndex);
					}
				}
			}
		}

		private ToggleObject GetActiveSelection()
		{
			foreach (var toggle in GetAllToggles())
			{
				if (toggle.isActive)
					return toggle;
			}

			return null;
		}

		private bool ValidateOperation()
		{
			foreach (var toggle in GetAllToggles())
			{
				if (toggle.isActive) return true;
			}

			return false;
		}

		private bool ValidateList(List<ToggleObject> toggleList)
		{
			foreach (var toggle in toggleList)
			{
				if (toggle.isActive) return true;
			}

			return false;
		}

		private ToggleObject GetRandom(List<ToggleObject> toggleObjects)
		{
			var rand = UnityEngine.Random.Range(0, toggleObjects.Count);
			if (toggleObjects[rand] != null && toggleObjects[rand].isActive)
			{
				return toggleObjects[rand];
			}
			else
			{
				return GetRandom(toggleObjects);
			}
		}
	}
}