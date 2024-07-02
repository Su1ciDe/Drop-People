using Fiber.Managers;
using Fiber.LevelSystem;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Fiber.Utilities
{
	/// <summary>
	/// For quickly testing specific levels in editor
	/// </summary>
	[InitializeOnLoad]
	public static class TestLevelEditor
	{
		private static string[] dropdown;

		static TestLevelEditor()
		{
			EditorApplication.playModeStateChanged += ModeChanged;
		}

		[InitializeOnLoadMethod]
		private static void ShowStartSceneButton()
		{
			ToolbarExtender.ToolbarExtender.RightToolbarGUI.Add(() =>
			{
				if (!LevelManager.Instance) return;

				GUI.enabled = !EditorApplication.isPlayingOrWillChangePlaymode;

				var gameSceneCount = LevelManager.Instance.Levels.Length;
				var tutorialSceneCount = LevelManager.Instance.TutorialLevels.Length;

				dropdown = new string[gameSceneCount + tutorialSceneCount + 1];
				dropdown[0] = "Play Level";
				int index = 1;

				for (int i = 1; i <= tutorialSceneCount; i++, index++)
					dropdown[index] = i + " - Tutorial Level";

				for (int i = 1; i <= gameSceneCount; i++, index++)
					dropdown[index] = i + " - Level";

				EditorGUI.BeginChangeCheck();
				int value = EditorGUILayout.Popup(0, dropdown, "Dropdown", GUILayout.Width(95));
				if (EditorGUI.EndChangeCheck())
				{
					if (value > 0)
					{
						LevelManager.Instance.LevelNo = value;

						EditorWindow.GetWindow(typeof(SceneView).Assembly.GetType("UnityEditor.GameView")).ShowNotification(new GUIContent("Testing Level " + value));
						EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

						framesToWaitUntilPlayMode = 0;
						EditorApplication.update -= EnterPlayMode;
						EditorApplication.update += EnterPlayMode;
					}
				}

				GUI.enabled = true;
			});
		}

		// Waits for the notification to be displayed if it's bigger than 0
		private static int framesToWaitUntilPlayMode;

		private static void EnterPlayMode()
		{
			if (framesToWaitUntilPlayMode-- <= 0)
			{
				EditorApplication.update -= EnterPlayMode;

				EditorPrefs.SetBool("TestingLevel", true);
				SetActiveLevels(false);
				EditorApplication.EnterPlaymode();
			}
		}

		private static void ModeChanged(PlayModeStateChange state)
		{
			// save scene before entering play mode
			if (state == PlayModeStateChange.ExitingEditMode)
			{
			}

			// variable for opening the active scene
			if (state == PlayModeStateChange.ExitingPlayMode)
			{
			}

			// opening the active scene
			if (state == PlayModeStateChange.EnteredEditMode)
			{
				if (EditorPrefs.GetBool("TestingLevel").Equals(true))
				{
					EditorPrefs.SetBool("TestingLevel", false);
					SetActiveLevels(true);
				}
			}
		}

		private static void SetActiveLevels(bool isActive)
		{
			var levels = Object.FindObjectsByType<Level>(FindObjectsSortMode.None);
			foreach (var level in levels)
				level.gameObject.SetActive(isActive);
		}
	}
}