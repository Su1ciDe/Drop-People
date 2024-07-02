using UnityEditor;
using UnityEngine;

namespace Fiber.Utilities
{
	/// <summary>
	/// Allows you to change TimeScale in the editor for testing purposes
	/// </summary>
	[InitializeOnLoad]
	public static class TimeScaleEditor
	{
		private struct TimeScaleType
		{
			public readonly string TimeScaleName;
			public readonly float TimeScaleAmount;

			public TimeScaleType(string name, float amount)
			{
				TimeScaleName = name;
				TimeScaleAmount = amount;
			}
		}

		private static readonly TimeScaleType[] types;
		private static readonly string[] dropdownItems;

		static TimeScaleEditor()
		{
			types = new[]
			{
				new TimeScaleType("x0.10", 0.1f), 
				new TimeScaleType("x0.25", 0.25f), 
				new TimeScaleType("x0.50", 0.5f), 
				new TimeScaleType("x1.00", 1f), 
				new TimeScaleType("x1.50", 1.50f), 
				new TimeScaleType("x2.00", 2f), 
				new TimeScaleType("x5.00", 5f), 
				new TimeScaleType("x10.00", 10f),
			};

			// Setup displayed items
			dropdownItems = new string[types.Length + 1];
			dropdownItems[0] = "Time Scale x" + Time.timeScale;
			for (int i = 1; i <= types.Length; i++)
				dropdownItems[i] = types[i - 1].TimeScaleName;
		}

		private const string ICON_PATH = "d_SpeedScale";

		public static class TimeScaleDropdown
		{
			[InitializeOnLoadMethod]
			private static void ShowTimeScaleDropDown()
			{
				ToolbarExtender.ToolbarExtender.LeftToolbarGUI.Add(() =>
				{
					dropdownItems[0] = "Time Scale x" + Time.timeScale;

					GUILayout.Space(10);

					EditorGUILayout.LabelField(EditorGUIUtility.IconContent(ICON_PATH), GUILayout.Width(26));
					EditorGUI.BeginDisabledGroup(!EditorApplication.isPlayingOrWillChangePlaymode);

					EditorGUI.BeginChangeCheck();
					int value = EditorGUILayout.Popup(0, dropdownItems, GUILayout.Width(120));
					if (EditorGUI.EndChangeCheck())
					{
						if (value > 0)
							SelectTimeScale(value);
					}

					EditorGUI.EndDisabledGroup();
				});
			}

			private static void SelectTimeScale(int value)
			{
				Time.timeScale = types[value - 1].TimeScaleAmount;
				dropdownItems[0] = "Time Scale x" + Time.timeScale;

				// Show a notification in scene
				foreach (SceneView scene in SceneView.sceneViews)
					scene.ShowNotification(new GUIContent("Time Scale: " + types[value - 1].TimeScaleAmount));
			}
		}
	}
}