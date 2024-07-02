using System.Collections;
using ElephantSdkManager;
using ElephantSdkManager.Util;
using ElephantSdkManager.Model;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Fiber.Build.ProjectConfig
{
	public static class SetupElephant
	{
#if UNITY_EDITOR
		public static IEnumerator SetupGamekitIDs(string elephantGameID)
		{
			if (elephantGameID.Equals("")) yield break;

			var gameKitDownloader = new UnityWebRequest(ManifestSource.GameKitURL + elephantGameID + "/config") { downloadHandler = new DownloadHandlerBuffer(), timeout = 240, };

			if (!string.IsNullOrEmpty(gameKitDownloader.error))
			{
				Debug.LogError("Unable to retrieve GameKit IDs.");
			}

			yield return gameKitDownloader.SendWebRequest();

			while (!gameKitDownloader.isDone)
			{
				yield return null;
			}

			var responseJson = gameKitDownloader.downloadHandler.text;

			if (string.IsNullOrEmpty(responseJson))
			{
				Debug.LogError("Unable to retrieve GameKit IDs.");
				yield break;
			}

			gameKitDownloader.Dispose();
			gameKitDownloader = null;
			var gameKitManifest = JsonUtility.FromJson<GameKitManifest>(responseJson);

			if (gameKitManifest?.data?.appKey == null)
			{
				Debug.LogError("Unable to retrieve GameKit IDs. Please set your IDs in RollicGames/RollicApplovinIDs.cs file!");
			}
			else
			{
				VersionUtils.SetupElephantThirdPartyIDs(gameKitManifest, "gamekit");
				VersionUtils.SetupGameKitIDs(gameKitManifest, "gamekit-max");
				AssetDatabase.Refresh();
			}
		}
#endif
	}
}