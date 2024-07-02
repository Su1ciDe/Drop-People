#if UNITY_EDITOR

using UnityEngine;

namespace FiberGames.LevelGenerator
{
    [System.Serializable]
    public class LevelBuildData
    {
        [SerializeField] public int maxLevelCounts = 0;

        [Range(0, 1)]
        [SerializeField] public float easyLevelRate = 0.5F;
        
        [Range(0, 1)]
        [SerializeField] public float normalLevelRate = 0.25F;

        [Range(0, 1)]
        [SerializeField] public float hardLevelRate = 0.25F;

        [Header("Level Creation Configurations")]
        [SerializeField] public LevelData easyLevelData = null;
        
        [SerializeField] public LevelData normalLevelData = null;

        [SerializeField] public LevelData hardLevelData = null;
        
        public int GetLevelCount(LevelTypes type)
        {
            var easyLevelCount = (int)(maxLevelCounts * easyLevelRate);

            var normalLevelCount = (int)(maxLevelCounts * normalLevelRate);

            var hardLevelCount = maxLevelCounts - easyLevelCount - normalLevelCount;
            
            switch (type)
            {
                case LevelTypes.Easy:
                    return easyLevelCount;
                
                case LevelTypes.Normal:
                    return normalLevelCount;
                
                case LevelTypes.Hard:
                    return hardLevelCount;
                default:
                    return -1;
            }
        }
        
        public string GetLevelName(int index)
        {
            var suffix = "";

            switch (GetLevelType(index))
            {
                case LevelTypes.Easy:
                {
                    suffix = " ( Easy ) ";

                    break;
                }

                case LevelTypes.Normal:
                {
                    suffix = " ( Normal ) ";
                    
                    break;
                }
                
                case LevelTypes.Hard:
                {
                    suffix = " ( Hard ) ";
                    
                    break;
                }
            }
            
            return "Level " + (index + 1) + suffix;
        }

        public LevelTypes GetLevelType(int index)
        {
            var limit = GetLevelCount(LevelTypes.Normal) +
                        GetLevelCount(LevelTypes.Easy);

            if (GetLevelCount(LevelTypes.Easy) > index)
                return LevelTypes.Easy;

            if (GetLevelCount(LevelTypes.Easy) <= index
                     &&
                     index < limit)
                return LevelTypes.Normal;

            if (limit <= index
                     &&
                     maxLevelCounts > index)
                return LevelTypes.Hard;

            return LevelTypes.Easy;
        }

        public LevelData GetLevelData(int index)
        {
            switch (GetLevelType(index))
            {
                case LevelTypes.Easy:
                    return easyLevelData;

                case LevelTypes.Normal:
                    return normalLevelData;

                case LevelTypes.Hard:
                    return hardLevelData;
            }

            return easyLevelData;
        }
        
        private float GetTotalLevelRate()
        {
            return easyLevelRate + normalLevelRate + hardLevelRate;
        }

        #region Validations

        public void ValidateData()
        {
            CheckBuildConfigurations();

            CheckLevelCreationConfigurations();
        }

        private void CheckBuildConfigurations()
        {
            if (GetTotalLevelRate() != 1)
            {
                easyLevelRate = 1F / 3F;
                
                normalLevelRate = 1F / 3F;

                hardLevelRate = 1 - easyLevelRate - normalLevelRate;
            }
        }

        private void CheckLevelCreationConfigurations()
        {
            easyLevelData.ValidateData();
            
            normalLevelData.ValidateData();
            
            hardLevelData.ValidateData();
        }

        #endregion
    }    
}

#endif