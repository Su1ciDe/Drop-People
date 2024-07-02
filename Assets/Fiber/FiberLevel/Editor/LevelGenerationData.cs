#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;

namespace FiberGames.LevelGenerator
{
    [System.Serializable]
    public class LevelGenerationData
    {
        [Header("@Level Parts")] 
        [SerializeField] public GameObject levelRoot = null;
        
        [SerializeField] public GameObject startPart = null;
        
        [SerializeField] public List<GameObject> easyParts = new List<GameObject>();
        
        [SerializeField] public List<GameObject> normalParts = new List<GameObject>();
        
        [SerializeField] public List<GameObject> hardParts = new List<GameObject>();
        
        [SerializeField] public GameObject finishPart = null;

        public void Clear()
        {
            easyParts.Clear();
            
            normalParts.Clear();
            
            hardParts.Clear();
        }
    }
}

#endif