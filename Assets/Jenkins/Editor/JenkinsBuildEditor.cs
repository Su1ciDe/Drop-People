// using Sirenix.OdinInspector.Editor;
// using Sirenix.Utilities;
// using Sirenix.Utilities.Editor;
// using UnityEditor;
//
//
// namespace AutoBuild.Editor
// {
//     
//     public class JenkinsBuildEditor : OdinEditorWindow
//     {
//         [MenuItem("Fiber Games/JenkinsBuildEditor",false, 20)]
//         private static void Open()
//         {
//             var window = GetWindow<ProductionEditorWindow>();
//             window.position = GUIHelper.GetEditorWindowRect().AlignCenter(500, 300);
//             
//         }
//         
//     }
// }


using UnityEngine;
using UnityEditor;
using System.Collections;
//using UnityEditor.AddressableAssets.Settings;


public class JenkinsBuildEditor : EditorWindow
{
    
    [MenuItem("Fiber/Jenkins Build",false, 50)]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(JenkinsBuildEditor), false, "JenkinsBuildEditor");
    }
   
    void OnGUI()
    {

        if(GUILayout.Button("TestBuildAddressable"))
        {
            TestBuildAddressable();
        }
        
    }

    public void TestBuildAddressable()
    {
        //AddressableAssetSettings.CleanPlayerContent();
        //AddressableAssetSettings.BuildPlayerContent();
    }
}
 
