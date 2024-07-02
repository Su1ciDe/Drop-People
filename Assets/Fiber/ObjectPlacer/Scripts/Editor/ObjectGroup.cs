using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Fiber.ObjectPlacer
{
    public class ObjectGroup : ScriptableObject
    {
        public string groupName;
        public List<GameObject> objectList = new List<GameObject>();
        public List<ToggleObject> toggleList = new List<ToggleObject>();
        public int initialOffset = 10;
        public int placementInterval = 40;
        public int elementPlacementInterval = 2;
        public Vector2 subGroupCountMinMax = new Vector2(1, 5);
        public bool useRandomX;
        public SerializedObject so;
        public SerializedProperty propGroupName;
        public SerializedProperty propObjectList;
        public SerializedProperty propInitialOffset;
        public SerializedProperty propPlacementInterval;
        public SerializedProperty propElementPlacementInterval;
        public SerializedProperty propGroupCountMinMax;
        public SerializedProperty propUseRandomX;
    
        public void Init(string name)
        {
            groupName = name;
            so = new SerializedObject(this);
            propObjectList = so.FindProperty("objectList");
            propGroupName = so.FindProperty("groupName");
            propInitialOffset = so.FindProperty("initialOffset");
            propPlacementInterval = so.FindProperty("placementInterval");
            propElementPlacementInterval = so.FindProperty("elementPlacementInterval");
            propGroupCountMinMax = so.FindProperty("subGroupCountMinMax");
            propUseRandomX = so.FindProperty("useRandomX");
        }
    
        public bool CheckChange()
        {
            if (objectList.Count != toggleList.Count)
            {
                toggleList.Clear();
                foreach (var obj in objectList)
                {
                    if (obj != null)
                    {
                        toggleList.Add(new ToggleObject(obj));
                    }
                }
                return true;
            }
            return false;
        }
    }
}