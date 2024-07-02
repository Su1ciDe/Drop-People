using UnityEditor;
#if UNITY_ANDROID
using System.Diagnostics;
using UnityEditor.Android;
#endif
using UnityEngine;

namespace Fiber.Build
{
	[CustomEditor(typeof(PlatformSwitcher))]
	public class PlatformSwitcherEditor : Editor
	{
		private static PlatformSwitcher platformSwitcher;

		private void OnEnable()
		{
			platformSwitcher ??= (PlatformSwitcher)target;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			if (GUILayout.Button("Switch"))
			{
				platformSwitcher.PerformSwitch();
#if UNITY_ANDROID
				SetupAndroidSDK33();
#endif
			}
		}
#if UNITY_ANDROID
		private static void SetupAndroidSDK33()
		{
			var sdkManagerPath = $"{AndroidExternalToolsSettings.sdkRootPath}/cmdline-tools/2.1/bin/sdkmanager";
			const string arguments = "\"platforms;android-33\"";

			var myProcess = new Process();
			var startInfo = Application.platform switch
			{
				RuntimePlatform.OSXEditor => new ProcessStartInfo()
				{
					FileName = sdkManagerPath,
					Arguments = arguments,
					UseShellExecute = false,
					CreateNoWindow = false,
					WindowStyle = ProcessWindowStyle.Hidden,
				},
				RuntimePlatform.WindowsEditor => new ProcessStartInfo()
				{
					FileName = sdkManagerPath,
					Arguments = arguments,
					UseShellExecute = true,
					Verb = "runas",
					CreateNoWindow = false,
					WindowStyle = ProcessWindowStyle.Hidden,
				},
				_ => null
			};

			myProcess.StartInfo = startInfo;
			myProcess.Start();
			myProcess.WaitForExit();
			myProcess.Dispose();

			AssetDatabase.Refresh();

			PlayerSettings.Android.targetSdkVersion = (AndroidSdkVersions)33;
		}
#endif
	}
}