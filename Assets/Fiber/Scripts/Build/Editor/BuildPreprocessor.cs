using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Fiber.Build
{
	public class BuildPreprocessor : IPreprocessBuildWithReport
	{
		public int callbackOrder => 0;

		public void OnPreprocessBuild(BuildReport report)
		{
			PlayerSettings.SplashScreen.show = true;
			PlayerSettings.SplashScreen.showUnityLogo = false;
		}
	}
}