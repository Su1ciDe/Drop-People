#if UNITY_EDITOR

using UnityEngine;

namespace FiberGames.LevelGenerator
{
    public class LevelPart : MonoBehaviour
    {
        [SerializeField] private LevelPartData data = null;

        public LevelPartData partData => data;
    }
}

#endif