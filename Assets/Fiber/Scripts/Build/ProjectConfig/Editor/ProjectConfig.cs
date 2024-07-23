using System.IO;
using System.Collections.Generic;
using ElephantSdkManager.Model;
using Facebook.Unity.Editor;
using Facebook.Unity.Settings;
using Fiber.UI;
using TriInspector;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Fiber.Build.ProjectConfig
{
	[DeclareTabGroup("tabs")]
	public class ProjectConfig : ScriptableObject
	{
		[Title("Player Settings")]
		[SerializeField] private string companyName = "Fiber Games";
		[SerializeField] private string productName;
		[SerializeField] private Texture2D productIcon;

		[PropertySpace(20)]
		[SerializeField] private string version = "0.1";
		[Group("tabs"), Tab("iOS Settings")] [SerializeField] private string iOSBundleId;
		[Group("tabs"), Tab("iOS Settings")] [SerializeField] private string teamId = "9JRFGT2VDU";
		// [Group("tabs"), Tab("iOS Settings")] [SerializeField] private string iOSVersion = "0.1";
		[Group("tabs"), Tab("iOS Settings")] [SerializeField] private string iOSBuildNo = "0";

		[Group("tabs"), Tab("Android Settings")] [SerializeField] private string androidBundleId;
		// [Group("tabs"), Tab("Android Settings")] [SerializeField] private string androidVersion = "0.1";
		[Group("tabs"), Tab("Android Settings")] [SerializeField] private int androidBuildNo;

		[Title("Facebook Settings")]
		[SerializeField] private string facebookId;
		[SerializeField] private string facebookClientToken;

		[Title("Elephant Settings")]
		[SerializeField] private string gameId;
		[SerializeField] private string gameSecret;

		[Title("Loading Screen")]
		[SerializeField] private Sprite background;
		[SerializeField] private Sprite loadingScreenLogo;
		[SerializeField] private Sprite loadingScreenTitle;

		private readonly Texture2D[] icons = new Texture2D[19];

		private ElephantSettings elephantSettings;
		private GameData elephantGameData;
		private FacebookSettings facebookSettings;

		private const string LOADING_SCREEN_PATH = "Assets/_Main/Prefabs/UserInterface/LoadingPanel.prefab";

#if UNITY_EDITOR
		private void OnEnable()
		{
			companyName = PlayerSettings.companyName = "Fiber Games";
		}

		public void Open()
		{
			LoadElephant();
			LoadFacebook();

			companyName = PlayerSettings.companyName;
			productName = PlayerSettings.productName;
			productIcon = PlayerSettings.GetIcons(NamedBuildTarget.Unknown, IconKind.Any)[0];

			// iOS
			iOSBundleId = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.iOS);
			teamId = PlayerSettings.iOS.appleDeveloperTeamID;
			iOSBuildNo = PlayerSettings.iOS.buildNumber;

			// Android 
			androidBundleId = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);
			androidBuildNo = PlayerSettings.Android.bundleVersionCode;

			// iOSVersion = androidVersion = PlayerSettings.bundleVersion;
			version = PlayerSettings.bundleVersion;

			facebookId = FacebookSettings.AppIds[0];
			facebookClientToken = FacebookSettings.ClientTokens[0];

			gameId = elephantSettings.GameID;
			gameSecret = elephantSettings.GameSecret;
		}

		[Button(ButtonSizes.Small, "Place Loading Screen")]
		private void LoadingScreen()
		{
			var loadingScreen = AssetDatabase.LoadAssetAtPath<GameObject>(LOADING_SCREEN_PATH);
			PrefabUtility.InstantiatePrefab(loadingScreen);
		}

		[Button(ButtonSizes.Large), PropertySpace(20)]
		public void Apply()
		{
			PlayerSettings.SplashScreen.show = true;
			PlayerSettings.SplashScreen.showUnityLogo = false;

			PlayerSettings.companyName = companyName;
			PlayerSettings.productName = productName;
			for (var i = 0; i < 19; i++)
				icons[i] = productIcon;

			PlayerSettings.SetIcons(NamedBuildTarget.iOS, icons, IconKind.Any);

			// iOS
			PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, iOSBundleId);
			PlayerSettings.iOS.appleDeveloperTeamID = teamId;
			PlayerSettings.iOS.buildNumber = iOSBuildNo;

			// Android 
			PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, androidBundleId);
			PlayerSettings.Android.bundleVersionCode = androidBuildNo;

			PlayerSettings.bundleVersion = version;

			FacebookSettings.AppIds = new List<string> { facebookId };
			FacebookSettings.ClientTokens = new List<string> { facebookClientToken };

			elephantSettings.GameID = gameId;
			elephantSettings.GameSecret = gameSecret;

			elephantGameData.gameId = gameId;

			EditorCoroutineUtility.StartCoroutine(SetupElephant.SetupGamekitIDs(gameId), this);

			SetLoadingScreen();

			ChangeReadMe();

			EditorUtility.SetDirty(elephantSettings);
			EditorUtility.SetDirty(facebookSettings);
			AssetDatabase.SaveAssets();
		}

		private void LoadElephant()
		{
			const string path = "Assets/Resources";

			if (!elephantSettings)
			{
				elephantSettings = Resources.Load<ElephantSettings>(nameof(ElephantSettings));
				if (!elephantSettings)
				{
					elephantSettings = ScriptableObject.CreateInstance<ElephantSettings>();
					if (!AssetDatabase.IsValidFolder(path))
						AssetDatabase.CreateFolder("Assets", "Resources");

					var assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/ElephantSettings.asset");
					AssetDatabase.CreateAsset(elephantSettings, assetPathAndName);

					AssetDatabase.SaveAssets();
					AssetDatabase.Refresh();
				}
			}

			if (!elephantGameData)
			{
				elephantGameData = Resources.Load<GameData>("GameData");
				if (!elephantGameData)
				{
					elephantGameData = ScriptableObject.CreateInstance<GameData>();
					if (!AssetDatabase.IsValidFolder(path))
						AssetDatabase.CreateFolder("Assets", "Resources");

					var assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/GameData.asset");
					AssetDatabase.CreateAsset(elephantGameData, assetPathAndName);

					AssetDatabase.SaveAssets();
					AssetDatabase.Refresh();
				}
			}
		}

		private void LoadFacebook()
		{
			if (facebookSettings) return;

			// This creates FacebookSettings if it's not created
			FacebookSettingsEditor.Edit();
			facebookSettings = FacebookSettings.Instance;
		}

		private void SetLoadingScreen()
		{
			var root = PrefabUtility.LoadPrefabContents(LOADING_SCREEN_PATH);

			if (!root)
			{
				Debug.LogWarning("No LoadingPanel prefab found at: " + LOADING_SCREEN_PATH);
				return;
			}

			var loadingPanel = root.GetComponent<LoadingPanelController>();

			EditorUtility.SetDirty(root);
			EditorSceneManager.MarkSceneDirty(root.scene);

			loadingPanel.SetLoadingScreen(background, loadingScreenLogo, loadingScreenTitle);

			PrefabUtility.SaveAsPrefabAsset(root, LOADING_SCREEN_PATH);
			EditorUtility.ClearDirty(root);
			PrefabUtility.UnloadPrefabContents(root);
		}

		/// <summary>
		/// Changes the name and icons in the readme file
		/// </summary>
		private void ChangeReadMe()
		{
			try
			{
				// Add icons here
				var iconList = new List<Object> { productIcon, loadingScreenLogo };

				const string readMeFileName = "README.md";
				if (!File.Exists(readMeFileName)) return;
				string path = Directory.GetCurrentDirectory();

				var lines = File.ReadAllLines(readMeFileName);
				int imgIndex = 0;

				for (var i = 0; i < lines.Length; i++)
				{
					if (lines[i].Contains("#") && !productName.Equals(""))
					{
						lines[i] = "# " + productName;
					}

					if (lines[i].Contains("img"))
					{
						if (iconList.Count > imgIndex && iconList[imgIndex])
						{
							lines[i] = "\t<img src=\"";
							lines[i] += AssetDatabase.GetAssetPath(iconList[imgIndex]);
							lines[i] += "\" width=\"200\">";
						}

						imgIndex++;
					}
				}

				File.WriteAllLines(path + "/" + readMeFileName, lines);
			}
			catch (IOException e)
			{
				Debug.LogError(e);
				throw;
			}
		}

		[MenuItem("Fiber/Project Config", false, 0)]
		public static void OpenProjectConfig()
		{
			const string path = "Assets/Fiber/Scripts/Build/ProjectConfig/GameConfig.asset";
			var projectConfig = AssetDatabase.LoadAssetAtPath<ProjectConfig>(path);

			if (!projectConfig)
			{
				projectConfig = CreateInstance<ProjectConfig>();
				AssetDatabase.CreateAsset(projectConfig, path);
			}

			projectConfig.Open();
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = projectConfig;
		}
#endif
	}
}