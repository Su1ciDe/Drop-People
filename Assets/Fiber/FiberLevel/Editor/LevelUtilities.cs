#if UNITY_EDITOR

using UnityEngine;

namespace FiberGames.LevelGenerator
{
    public class LevelUtilities : MonoBehaviour
    {
        public static bool RandomBool(float threshold = 0)
        {
            var weight = Random.Range(0.0F, 1F);

            return weight > 1 - threshold;
        }
    }
}

#endif