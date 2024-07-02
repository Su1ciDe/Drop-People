using System.Collections.Generic;
using UnityEngine;

namespace Fiber.Utilities.Extensions
{
	public static class TransformExtensions
	{
		/// <summary>
		/// It tells you whether this transform is inside a camera.
		/// </summary>
		/// <param name="camera">Camera</param>
		/// <returns>Is inside or not</returns>
		public static bool IsInsideCamera(this Transform t, Camera camera = null)
		{
			return Helper.IsPositionInsideCamera(t.position, camera);
		}

		/// <summary>
		/// Destroy all children of this parent
		/// </summary>
		public static void DestroyChildren(this Transform parent)
		{
			var childCount = parent.childCount;
			for (int i = 0; i < childCount; i++)
				UnityEngine.Object.Destroy(parent.GetChild(0).gameObject);
		}

		/// <summary>
		/// DestroyImmediate all children of this parent
		/// </summary>
		public static void DestroyImmediateChildren(this Transform parent)
		{
			var childCount = parent.childCount;
			for (int i = 0; i < childCount; i++)
				UnityEngine.Object.DestroyImmediate(parent.GetChild(0).gameObject);
		}

		/// <summary>
		/// Disable all children of this parent
		/// </summary>
		public static void DisableChildren(this Transform parent)
		{
			foreach (Transform t in parent)
				t.gameObject.SetActive(false);
		}

		/// <summary>
		/// Set parent's and all children's layer to given layer
		/// <br/><i>Note that this code is intensive because it's recursive</i>
		/// </summary>
		/// <param name="layer">The layer you want to set</param>
		public static void SetChildrenLayer(this Transform parent, int layer)
		{
			parent.gameObject.layer = layer;
			foreach (Transform t in parent)
				SetChildrenLayer(t, layer);
		}

		/// <summary>
		/// Returns a value indicating whether this instance is not equal to a specified transform.
		/// </summary>
		/// <param name="other">A transform to compare to this instance.</param>
		public static bool NotEquals(this Transform t, Transform other) => !t.Equals(other);

		/// <summary>
		///  A flexible way to search for a child within a parent hierarchy
		/// </summary>
		/// <param name="childName">Searched child transform's name</param>
		/// <param name="type">The methods utilize two different search algorithms: breadth-first search and depth-first search.</param>
		/// <returns>The child transform</returns>
		public static Transform FindDeepChild(this Transform parent, string childName, GraphSearchType type = GraphSearchType.BreadthFirstSearch)
		{
			if (type == GraphSearchType.BreadthFirstSearch)
			{
				var queue = new Queue<Transform>();
				queue.Enqueue(parent);
				while (queue.Count > 0)
				{
					var c = queue.Dequeue();
					if (c.name.Equals(childName))
						return c;
					foreach (Transform t in c)
						queue.Enqueue(t);
				}

				return null;
			}
			else if (type == GraphSearchType.DepthFirstSearch)
			{
				foreach (Transform child in parent)
				{
					if (child.name.Equals(childName))
						return child;
					var result = child.FindDeepChild(childName, GraphSearchType.DepthFirstSearch);
					if (result)
						return result;
				}

				return null;
			}

			return null;
		}
	}

	public enum GraphSearchType
	{
		/// <summary>
		/// Explores a graph level by level.
		/// It systematically visits all nodes at the current level before moving to the next level.
		/// </summary>
		BreadthFirstSearch,
		/// <summary>
		/// Explores a graph branch by branch.
		/// It starts at the root (or a specified node) and explores as far as possible along each branch before backtracking.
		/// </summary>
		DepthFirstSearch
	}
}