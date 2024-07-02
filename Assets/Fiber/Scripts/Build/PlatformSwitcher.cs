using UnityEditor;
using UnityEngine;

namespace Fiber.Build
{
	[ExecuteInEditMode]
	public class PlatformSwitcher : MonoBehaviour
	{
#if UNITY_EDITOR
		[SerializeField] private BuildTarget buildTarget = BuildTarget.iOS;

		public void PerformSwitch()
		{
			if (EditorUserBuildSettings.activeBuildTarget == buildTarget) return;

			EditorUserBuildSettings.SwitchActiveBuildTarget(BuildPipeline.GetBuildTargetGroup(buildTarget), buildTarget);

			switch (buildTarget)
			{
				case BuildTarget.iOS:
					SetupIOS();
					break;
				case BuildTarget.Android:
					SetupAndroid();
					break;
			}
		}

		private void SetupAndroid()
		{
#if UNITY_ANDROID
			PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
			PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Android, ApiCompatibilityLevel.NET_2_0);
			PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
#endif
		}

		private void SetupIOS()
		{
#if UNITY_IOS
			PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.iOS, ApiCompatibilityLevel.NET_Standard);
#endif
		}
#endif
	}
}