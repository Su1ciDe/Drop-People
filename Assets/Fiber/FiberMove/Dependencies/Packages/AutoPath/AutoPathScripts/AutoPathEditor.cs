#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AutoPathGenerator))]
public class AutoPathEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        AutoPathGenerator pathGenerator = (AutoPathGenerator)target;
        if(GUILayout.Button("Generate Path"))
        {
            pathGenerator.GeneratePath();
        }
    }
}

#endif