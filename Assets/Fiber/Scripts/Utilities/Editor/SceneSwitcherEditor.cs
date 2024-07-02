using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fiber.Utilities
{
	/// <summary>
	/// For quickly switching a scene
	/// </summary>
	[InitializeOnLoad]
	public static class SceneSwitcherEditor
	{
		private static readonly Dictionary<int, string> dropdown = new Dictionary<int, string>();

		[InitializeOnLoadMethod]
		private static void ShowStartSceneButton()
		{
			ToolbarExtender.ToolbarExtender.LeftToolbarGUI.Add(() =>
			{
				GUI.enabled = !EditorApplication.isPlayingOrWillChangePlaymode;

				var sceneCount = SceneManager.sceneCountInBuildSettings;

				dropdown[0] = "Scene Switcher";
				int index = 1;
				for (int i = 0; i < sceneCount; i++, index++)
					dropdown[index] = SceneUtility.GetScenePathByBuildIndex(i);

				EditorGUI.BeginChangeCheck();
				int value = EditorGUILayout.Popup(0, dropdown.Values.Select(System.IO.Path.GetFileNameWithoutExtension).ToArray(), "Dropdown", GUILayout.Width(125));
				if (EditorGUI.EndChangeCheck())
				{
					if (value > 0)
					{
						EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
						EditorSceneManager.OpenScene(dropdown[value], OpenSceneMode.Single);
					}
				}

				GUI.enabled = true;
			});
		}
	}
}