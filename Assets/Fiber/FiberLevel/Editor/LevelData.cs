#if UNITY_EDITOR

using UnityEngine;

namespace FiberGames.LevelGenerator
{
    // [CreateAssetMenu]
    public class LevelData : ScriptableObject
    {
        [SerializeField] public int minLevelLength = 3;

        [SerializeField] public int maxLevelLength = 10;
        
        [Range(0, 1)]
        [SerializeField] public float easyPartRate = 0.5F;
        
        [Range(0, 1)]
        [SerializeField] public float normalPartRate = 0.25F;

        [Range(0, 1)]
        [SerializeField] public float hardPartRate = 0.25F;
        
        private float GetTotalLevelRate()
        {
            return easyPartRate + normalPartRate + hardPartRate;
        }

        public void ValidateData()
        {
            CheckBuildConfigurations();
        }

        private void CheckBuildConfigurations()
        {
            if (GetTotalLevelRate() != 1)
            {
                easyPartRate = 1F / 3F;
                
                normalPartRate = 1F / 3F;

                hardPartRate = 1 - easyPartRate - normalPartRate;
            }
        }
    }    
}

#endif