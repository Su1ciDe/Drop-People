using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Fiber.Utilities
{
	public static class EditorUtilities
	{
		public static T CreateVisualElement<T>(params string[] classNames) where T : VisualElement, new()
		{
			var element = new T();
			foreach (var className in classNames)
				element.AddToClassList(className);

			return element;
		}

#if UNITY_EDITOR

		/// <summary>
		/// Returns all the assets object of type at given path.
		/// </summary>
		/// <param name="path">Folder's path</param>
		public static IEnumerable<T> LoadAllAssetsFromPath<T>(string path) where T : Object
		{
			var filePaths = System.IO.Directory.GetFiles(path);

			foreach (var filePath in filePaths)
			{
				var obj = AssetDatabase.LoadAssetAtPath(filePath, typeof(T));
				if (obj is T asset)
					yield return asset;
			}
		}

		/// <summary>
		/// Searches the asset database and returns all of the objects of type at given paths
		/// </summary>
		/// <param name="paths">The folders where the search will start.</param>
		public static IEnumerable<T> FindAllAssetsAtPath<T>(params string[] paths) where T : Object
		{
			var assetGUIDs = AssetDatabase.FindAssets("t:Object", paths);
			foreach (var guid in assetGUIDs)
			{
				var myObjectPath = AssetDatabase.GUIDToAssetPath(guid);
				var myObjs = AssetDatabase.LoadAllAssetsAtPath(myObjectPath);
				foreach (var myObj in myObjs)
				{
					if (myObj is T)
						yield return AssetDatabase.LoadAssetAtPath<T>(myObjectPath);
				}
			}
		}

		/// <summary>
		/// Finds the first type of object in which you specified 
		/// </summary>
		/// <param name="obj">Instanced object</param>
		/// <param name="paths">Searched paths</param>
		/// <returns>The Asset</returns>
		public static T FindObjectFromInstance<T>(T obj, params string[] paths) where T : Object
		{
			string type = typeof(T).ToString().Replace("UnityEngine.", "");
			var guIds = AssetDatabase.FindAssets($"{obj.name} t:{type}", paths);
			if (guIds.Length <= 0) return null;

			var assetPath = AssetDatabase.GUIDToAssetPath(guIds[0]);
			return AssetDatabase.LoadAssetAtPath<T>(assetPath);
		}
#endif
	}
}