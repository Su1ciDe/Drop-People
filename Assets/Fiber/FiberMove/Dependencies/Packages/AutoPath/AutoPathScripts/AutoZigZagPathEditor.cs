#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AutoZigZagPathGenerator))]
public class AutoZigZagPathEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        AutoZigZagPathGenerator pathGenerator = (AutoZigZagPathGenerator)target;
        if(GUILayout.Button("Generate Path"))
        {
            pathGenerator.GeneratePath();
        }
    }
}

#endif