#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FiberGames.LevelGenerator
{
    public class FiberLevel : EditorWindow
    {
        private Transform levelParent = null;
        
        private GameObject levelRoot = null;
            
        private GameObject startPart = null;
        
        private GameObject finishPart = null;

        private int easyPartsLimit = 0;
        
        private int normalPartsLimit = 0;

        private int hardPartsLimit = 0;
        
        private GameObject[] easyParts = new GameObject[0];
        
        private GameObject[] normalParts = new GameObject[0];
        
        private GameObject[] hardParts = new GameObject[0];

        private int maxLevelCounts = 0;

        private float easyLevelRate = 0.5F;

        private float normalLevelRate = 0.25F;

        private float hardLevelRate = 0.25F;

        private LevelData easyLevelData = null;
        
        private LevelData normalLevelData = null;

        private LevelData hardLevelData = null;
        
        private Vector2 scrollPos = Vector2.zero;

        private GameObject initializeElement = null;
        
        [MenuItem("Fiber/Runner/Level Generator",false,999)]
        public static void ShowWindow()
        {
            GetWindow(typeof(FiberLevel), false, "Fiber Level"); //GetWindow is a method inherited from the EditorWindow class
        }
        
        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            
            scrollPos = GUILayout.BeginScrollView(scrollPos, false, false, GUIStyle.none, GUI.skin.verticalScrollbar);
            
            GUILayout.Label("", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox); //Declaring our first part of our layout, and adding a bit of flare with EditorStyles.

            GUILayout.Label("Level Settings", EditorStyles.boldLabel);

            levelParent = EditorGUILayout.ObjectField("Level Parent", levelParent, typeof(Transform), true) as Transform;
            
            EditorGUILayout.EndVertical(); // And closing our last area.

            GUILayout.Label("", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox); //Declaring our first part of our layout, and adding a bit of flare with EditorStyles.

            GUILayout.Label("Level Prefabs", EditorStyles.boldLabel);

            levelRoot = EditorGUILayout.ObjectField("Level Root", levelRoot, typeof(GameObject), true) as GameObject;
            
            startPart = EditorGUILayout.ObjectField("Start Part", startPart, typeof(GameObject), true) as GameObject;
            
            finishPart = EditorGUILayout.ObjectField("Finish Part", finishPart, typeof(GameObject), true) as GameObject;
            
            EditorGUILayout.EndVertical(); // And closing our last area.

            GUILayout.Label("", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox); //Declaring our first part of our layout, and adding a bit of flare with EditorStyles.
            
            var easyLimit = EditorGUILayout.IntField("Easy Parts", easyPartsLimit);
            
            easyPartsLimit = easyLimit < 0 ? 0 : easyLimit;

            if (easyParts.Length != easyPartsLimit)
            {
                if (easyParts.Length > easyPartsLimit)
                {
                    var parts = new List<GameObject>();

                    for (var i = 0; i < easyPartsLimit; i++)
                    {
                        parts.Add(easyParts[i]);
                    }

                    easyParts = parts.ToArray();
                }

                else
                {
                    var parts = easyParts.ToList();

                    for (var i = 0; i < easyPartsLimit - easyParts.Length; i++)
                    {
                        parts.Add(null);
                    }

                    easyParts = parts.ToArray();
                }
            }

            for (var i = 0; i < easyPartsLimit; i++) //Your loop to display the objects, THIS must exist in OnGUI.
            {
                easyParts[i] = EditorGUILayout.ObjectField(easyParts[i], typeof(GameObject), true) as GameObject;
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(15);
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndVertical(); // And closing our last area.
            
            GUILayout.Label("", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox); //Declaring our first part of our layout, and adding a bit of flare with EditorStyles.
            
            var normalLimit = EditorGUILayout.IntField("Normal Parts", normalPartsLimit);
            
            normalPartsLimit = normalLimit < 0 ? 0 : normalLimit;

            if (normalParts.Length != normalPartsLimit)
            {
                if (normalParts.Length > normalPartsLimit)
                {
                    var parts = new List<GameObject>();

                    for (var i = 0; i < normalPartsLimit; i++)
                    {
                        parts.Add(normalParts[i]);
                    }

                    normalParts = parts.ToArray();
                }

                else
                {
                    var parts = normalParts.ToList();

                    for (var i = 0; i < normalPartsLimit - normalParts.Length; i++)
                    {
                        parts.Add(null);
                    }

                    normalParts = parts.ToArray();
                }
            }

            for (var i = 0; i < normalPartsLimit; i++) //Your loop to display the objects, THIS must exist in OnGUI.
            {
                normalParts[i] = EditorGUILayout.ObjectField(normalParts[i], typeof(GameObject), true) as GameObject;
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(15);
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndVertical(); // And closing our last area.
            
            GUILayout.Label("", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox); //Declaring our first part of our layout, and adding a bit of flare with EditorStyles.

            var hardLimit = EditorGUILayout.IntField("Hard Parts", hardPartsLimit);
            
            hardPartsLimit = hardLimit < 0 ? 0 : hardLimit;

            if (hardParts.Length != hardPartsLimit)
            {
                if (hardParts.Length > hardPartsLimit)
                {
                    var parts = new List<GameObject>();

                    for (var i = 0; i < hardPartsLimit; i++)
                    {
                        parts.Add(hardParts[i]);
                    }

                    hardParts = parts.ToArray();
                }

                else
                {
                    var parts = hardParts.ToList();

                    for (var i = 0; i < hardPartsLimit - hardParts.Length; i++)
                    {
                        parts.Add(null);
                    }

                    hardParts = parts.ToArray();
                }
            }


            for (var i = 0; i < hardPartsLimit; i++) //Your loop to display the objects, THIS must exist in OnGUI.
            {
                hardParts[i] = EditorGUILayout.ObjectField(hardParts[i], typeof(GameObject), true) as GameObject;
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(15);
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndVertical(); // And closing our last area.
            
            GUILayout.Label("", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox); //Declaring our first part of our layout, and adding a bit of flare with EditorStyles.
            
            GUILayout.Label("Level Build", EditorStyles.boldLabel);

            var maxCount = EditorGUILayout.IntField("Max Level Counts", maxLevelCounts);

            maxLevelCounts = maxCount < 0 ? 0 : maxCount; 

            easyLevelRate = EditorGUILayout.Slider("Easy Level Rate", easyLevelRate, 0F, 1F);
            
            normalLevelRate = EditorGUILayout.Slider("Normal Level Rate", normalLevelRate, 0F, 1F);

            hardLevelRate = EditorGUILayout.Slider("Hard Level Rate", hardLevelRate, 0F, 1F);

            EditorGUILayout.EndVertical(); // And closing our last area.

            GUILayout.Label("", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox); //Declaring our first part of our layout, and adding a bit of flare with EditorStyles.
            
            GUILayout.Label("Level Creation Configurations", EditorStyles.boldLabel);

            easyLevelData = EditorGUILayout.ObjectField("Easy Level Data", easyLevelData, typeof(LevelData), true) as LevelData;
            
            normalLevelData = EditorGUILayout.ObjectField("Normal Level Data", normalLevelData, typeof(LevelData), true) as LevelData;

            hardLevelData = EditorGUILayout.ObjectField("Hard Level Data", hardLevelData, typeof(LevelData), true) as LevelData;

            EditorGUILayout.EndVertical(); // And closing our last area.
            
            GUILayout.Label("", EditorStyles.boldLabel);

            if (initializeElement == null)
            {
                GUI.backgroundColor = Color.yellow;
                if (GUILayout.Button("Setup Fiber Level"))
                {
                    var prefab = PrefabUtility.LoadPrefabContents("Assets/Fiber/FiberLevel/Prefabs/LevelCoreParts/FiberLevel.prefab");

                    initializeElement = Instantiate(prefab, null);

                    initializeElement.name = "FiberLevel";
                }
            }

            if (!(initializeElement == null))
            {
                GUI.backgroundColor = Color.green;
                if (GUILayout.Button("Create Levels"))
                {
                    var generationData = new LevelGenerationData
                    {
                        levelRoot = levelRoot,
                        startPart = startPart,
                        easyParts = easyParts.ToList(),
                        normalParts = normalParts.ToList(),
                        hardParts = hardParts.ToList(),
                        finishPart = finishPart
                    };

                    var buildData = new LevelBuildData
                    {
                        maxLevelCounts = maxLevelCounts,
                        easyLevelRate = easyLevelRate,
                        normalLevelRate = normalLevelRate,
                        hardLevelRate = hardLevelRate,
                        easyLevelData = easyLevelData,
                        normalLevelData = normalLevelData,
                        hardLevelData = hardLevelData
                    };
                
                    LevelGenerator.Instance.ApplyData(levelParent, levelParent, generationData, buildData);
                
                    LevelGenerator.Instance.CreateLevels();
                };
            
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("Clear Levels"))
                {
                    var levels = levelParent.GetComponentsInChildren<Transform>();
                    
                    for (var i = levels.Length - 1; i >= 1; i--)
                    {
                        DestroyImmediate(levels[i].gameObject);
                    }
                
                    LevelGenerator.Instance.ClearLevels(true);
                }
            }
            
            GUILayout.Label("", EditorStyles.boldLabel);

            GUI.contentColor = Color.gray;
            GUILayout.Label("FİBER LEVEL v0.1 [RELEASE BUILD]", EditorStyles.boldLabel);

            EditorGUILayout.EndScrollView();
            
            EditorGUILayout.EndVertical();
        }

        private void OnDestroy()
        {
            DestroyImmediate(initializeElement);
        }
    }
}

#endif