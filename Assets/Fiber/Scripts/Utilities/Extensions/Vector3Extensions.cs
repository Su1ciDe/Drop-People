using System.Collections.Generic;
using UnityEngine;

namespace Fiber.Utilities.Extensions
{
	public static class Vector3Extensions
	{
		/// <summary>
		/// X and Y component of the vector
		/// </summary>
		/// <returns>X and Y</returns>
		public static Vector3 xy(this Vector3 vector) => new Vector3(vector.x, vector.y, 0);

		/// <summary>
		/// X and Z component of the vector
		/// </summary>
		/// <returns>X and Z</returns>
		public static Vector3 xz(this Vector3 vector) => new Vector3(vector.x, 0, vector.z);

		/// <summary>
		/// Y and Z component of the vector
		/// </summary>
		/// <returns>Y and Z</returns>
		public static Vector3 yz(this Vector3 vector) => new Vector3(0, vector.y, vector.z);

		/// <summary>
		/// Sets X value of this vector
		/// </summary>
		/// <param name="x">X value</param>
		public static void SetX(ref this Vector3 vector, float x)
		{
			vector.Set(x, vector.y, vector.z);
		}

		/// <summary>
		/// Sets Y value of this vector
		/// </summary>
		/// <param name="y">Y value</param>
		public static void SetY(ref this Vector3 vector, float y)
		{
			vector.Set(vector.x, y, vector.z);
		}

		/// <summary>
		/// Sets Z value of this vector
		/// </summary>
		/// <param name="z">Z value</param>
		public static void SetZ(ref this Vector3 vector, float z)
		{
			vector.Set(vector.x, vector.y, z);
		}

		/// <summary>
		/// Converts a Vector3 array to Vector2 array
		/// </summary>
		/// <returns>Vector2 array</returns>
		public static Vector2[] ToVector2(this Vector3[] vectors)
		{
			var count = vectors.Length;
			var v2 = new Vector2[count];
			for (int i = 0; i < count; i++)
				v2[i] = new Vector2(vectors[i].x, vectors[i].y);
			return v2;
		}

		/// <summary>
		/// Converts a Vector3 list to Vector2 list
		/// </summary>
		/// <returns>Vector2 list</returns>
		public static List<Vector2> ToVector2(this List<Vector3> vectors)
		{
			var count = vectors.Count;
			var v2 = new List<Vector2>(count);
			for (int i = 0; i < count; i++)
				v2[i] = new Vector2(vectors[i].x, vectors[i].y);
			return v2;
		}

		/// <summary>
		/// Returns a value indicating whether this instance is not equal to a specified vector.
		/// </summary>
		/// <param name="other">A vector to compare to this instance.</param>
		public static bool NotEquals(this Vector3 vector, Vector3 other) => !vector.Equals(other);
	}
}