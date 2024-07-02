#if UNITY_EDITOR

using System.Collections.Generic;
using Fiber.Utilities;
using UnityEditor;
using UnityEngine;

namespace FiberGames.LevelGenerator
{
    public enum LevelTypes
    {
        Easy,
        Normal,
        Hard
    }
    
    public class LevelGenerator : SingletonInit<LevelGenerator>
    {
        [SerializeField] private Transform levelOffset = null;

        [SerializeField] private Transform levelParent = null;
        
        [SerializeField] private LevelGenerationData levelPrefabs = null;
        
        [SerializeField] private LevelBuildData levelBuild = null;
        
        public void ApplyData(Transform offset, Transform parent, LevelGenerationData generationData, 
            LevelBuildData buildData)
        {
            ClearLevels();
            
            levelOffset = offset;

            levelParent = parent;

            levelPrefabs = generationData;

            levelBuild = buildData;

            levelUniqueData = new LevelUniqueData();
        }

        #region Validations

        private void ValidateConfigurations()
        {
            levelBuild.ValidateData();

            levelUniqueData.ValidateData(levelPrefabs.easyParts.Count, 
                levelPrefabs.normalParts.Count, levelPrefabs.hardParts.Count);
        }

        #endregion Validations

        #region LevelGeneration

        private int _levelPartCount = 0;
        
        private int _levelPartIndex = 0;

        private int _levelIndex = 0;

        private string _levelName = "";

        private string _levelId = "";

        private string _lastCreatedPartId = "";

        private LevelPart _lastCreatedPart = null;

        public void ClearLevels(bool shouldRemoveFiles = false)
        {
            levelPrefabs.Clear();
            
            levelUniqueData.Clear();
            
            levelIds.Clear();

            if (shouldRemoveFiles)
            {
                FileUtil.DeleteFileOrDirectory("Assets/FiberLevel/Prefabs/Levels/");
                
                var guid = AssetDatabase.CreateFolder("Assets/FiberLevel/Prefabs","Levels");
                
                AssetDatabase.GUIDToAssetPath(guid);
                
                AssetDatabase.Refresh();
            }
        }
        
        public void CreateLevels()
        {
            ValidateConfigurations();
            
            for (var i = 0; i < levelBuild.maxLevelCounts; i++)
            {
                _levelIndex = i;

                GenerateLevel();
            }
        }

        private void GenerateLevel()
        {
            OnStartLevelGeneration();
            
            var level = Instantiate(levelPrefabs.levelRoot, levelParent);

            var levelPos = Vector3.zero;
            
            level.name = _levelName;

            GameObject levelObject = null;
            
            for (var i = 0; i < _levelPartCount; i++)
            {
                if (i == 0)
                {
                    levelObject = Instantiate(DetermineNextLevelPart(), Vector3.zero, transform.rotation);

                    levelPos = levelOffset.position + Vector3.right * (_levelIndex * 10);
                }
                    
                else
                    levelObject = Instantiate(DetermineNextLevelPart(), _lastCreatedPart.partData.jointTransform.position, transform.rotation);
                
                levelObject.transform.SetParent(level.transform);

                _lastCreatedPart = levelObject.GetComponent<LevelPart>();

                _levelId += _lastCreatedPartId;
            }

            PrefabUtility.SaveAsPrefabAsset(level, "Assets/FiberLevel/Prefabs/Levels/" + level.name + ".prefab");

            level.transform.position = levelPos;

            if (!IsUnique(_levelId))
            {
                DestroyImmediate(level);
            }

            else
            {
                AddUniqueId(_levelId);
            }

            OnCompleteLevelGeneration();
        }

        private GameObject DetermineNextLevelPart()
        {
            GameObject part = null;
            
            if (_levelPartIndex == 0)
            {
                part = levelPrefabs.startPart;
            }
            
            else if (_levelPartIndex == _levelPartCount - 1)
            {
                part = levelPrefabs.finishPart;
            }

            else
            {
                switch (levelBuild.GetLevelType(_levelIndex))
                {
                    case LevelTypes.Easy:
                    {
                        if (LevelUtilities.RandomBool(levelBuild.easyLevelData.easyPartRate))
                        {
                            var index = Random.Range(0, levelPrefabs.easyParts.Count);

                            _lastCreatedPartId = levelUniqueData.easyPartIds[index];
                            
                            part = levelPrefabs.easyParts[index];
                        }
                        
                        else if (LevelUtilities.RandomBool(levelBuild.easyLevelData.normalPartRate))
                        {
                            var index = Random.Range(0, levelPrefabs.normalParts.Count);

                            _lastCreatedPartId = levelUniqueData.normalPartIds[index];
                            
                            part = levelPrefabs.normalParts[index];
                        }
                        
                        else if (LevelUtilities.RandomBool(levelBuild.easyLevelData.hardPartRate))
                        {
                            var index = Random.Range(0, levelPrefabs.hardParts.Count);

                            _lastCreatedPartId = levelUniqueData.hardPartIds[index];
                            
                            part = levelPrefabs.hardParts[index];
                        }
                        
                        if (part == null)
                        {
                            var index = Random.Range(0, levelPrefabs.easyParts.Count);

                            _lastCreatedPartId = levelUniqueData.easyPartIds[index];
                            
                            part = levelPrefabs.easyParts[index];
                        }
                        
                        break;
                    }
                    
                    case LevelTypes.Normal:
                    {
                        if (LevelUtilities.RandomBool(levelBuild.normalLevelData.easyPartRate))
                        {
                            var index = Random.Range(0, levelPrefabs.easyParts.Count);

                            _lastCreatedPartId = levelUniqueData.easyPartIds[index];
                            
                            part = levelPrefabs.easyParts[index];
                        }
                        
                        else if (LevelUtilities.RandomBool(levelBuild.normalLevelData.normalPartRate))
                        {
                            var index = Random.Range(0, levelPrefabs.normalParts.Count);

                            _lastCreatedPartId = levelUniqueData.normalPartIds[index];
                            
                            part = levelPrefabs.normalParts[index];
                        }
                        
                        else if (LevelUtilities.RandomBool(levelBuild.normalLevelData.hardPartRate))
                        {
                            var index = Random.Range(0, levelPrefabs.hardParts.Count);

                            _lastCreatedPartId = levelUniqueData.hardPartIds[index];
                            
                            part = levelPrefabs.hardParts[index];
                        }
                        
                        if (part == null)
                        {
                            var index = Random.Range(0, levelPrefabs.normalParts.Count);

                            _lastCreatedPartId = levelUniqueData.normalPartIds[index];
                            
                            part = levelPrefabs.normalParts[index];
                        }
                        
                        break;
                    }
                    
                    case LevelTypes.Hard:
                    {
                        if (LevelUtilities.RandomBool(levelBuild.hardLevelData.easyPartRate))
                        {
                            var index = Random.Range(0, levelPrefabs.easyParts.Count);

                            _lastCreatedPartId = levelUniqueData.easyPartIds[index];
                            
                            part = levelPrefabs.easyParts[index];
                        }
                        
                        else if (LevelUtilities.RandomBool(levelBuild.hardLevelData.normalPartRate))
                        {
                            var index = Random.Range(0, levelPrefabs.normalParts.Count);

                            _lastCreatedPartId = levelUniqueData.normalPartIds[index];
                            
                            part = levelPrefabs.normalParts[index];
                        }
                        
                        else if (LevelUtilities.RandomBool(levelBuild.hardLevelData.hardPartRate))
                        {
                            var index = Random.Range(0, levelPrefabs.hardParts.Count);

                            _lastCreatedPartId = levelUniqueData.hardPartIds[index];
                            
                            part = levelPrefabs.hardParts[index];
                        }

                        if (part == null)
                        {
                            var index = Random.Range(0, levelPrefabs.hardParts.Count);

                            _lastCreatedPartId = levelUniqueData.hardPartIds[index];
                            
                            part = levelPrefabs.hardParts[index];
                        }
                        
                        break;
                    }
                }
            }

            _levelPartIndex++;
            
            return part;
        }

        private void OnStartLevelGeneration()
        {
            _levelPartCount = Random.Range(levelBuild.GetLevelData(_levelIndex).minLevelLength,
                levelBuild.GetLevelData(_levelIndex).maxLevelLength);
            
            _levelName = levelBuild.GetLevelName(_levelIndex);
        }

        private void OnCompleteLevelGeneration()
        {
            _levelPartIndex = 0;

            _levelPartCount = 0;

            _levelName = "";

            _levelId = "";

            _lastCreatedPartId = "";
            
            _lastCreatedPart = null;
        }

        #endregion LevelGeneration

        #region UniqueLevel

        [Header("@Debug")]
        
        [SerializeField] private LevelUniqueData levelUniqueData = null;

        [SerializeField] private List<string> levelIds = new List<string>();

        private bool IsUnique(string id)
        {
            foreach (var levelId in levelIds)
            {
                if (levelId == id) return false;
            }

            return true;
        }

        private void AddUniqueId(string newId)
        {
            levelIds.Add(newId);
        }

        #endregion
    }
}

#endif