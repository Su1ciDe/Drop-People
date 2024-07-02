#if UNITY_EDITOR

using System;
using System.Collections.Generic;

namespace FiberGames.LevelGenerator
{
    [Serializable]
    public class LevelUniqueData
    {
        public List<string> easyPartIds = new List<string>();

        public List<string> normalPartIds = new List<string>();

        public List<string> hardPartIds = new List<string>();

        public void ValidateData(int easyPartLength, int normalPartLength, int hardPartLength)
        {
            for (var i = 0; i < easyPartLength; i++)
            {
                easyPartIds.Add(Guid.NewGuid().ToString("N"));
            }
            
            for (var i = 0; i < normalPartLength; i++)
            {
                normalPartIds.Add(Guid.NewGuid().ToString("N"));
            }
            
            for (var i = 0; i < hardPartLength; i++)
            {
                hardPartIds.Add(Guid.NewGuid().ToString("N"));
            }
        }

        public void Clear()
        {
            easyPartIds.Clear();
            
            normalPartIds.Clear();
            
            hardPartIds.Clear();
        }
    }    
}

#endif