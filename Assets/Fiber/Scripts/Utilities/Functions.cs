using UnityEngine;

namespace Fiber.Utilities
{
	public static class Functions
	{
		/// <summary>
		/// Remaps a number from one range to another.
		/// </summary>
		/// <param name="value">The incoming value to be converted</param>
		/// <param name="valueMin">Lower bound of the value's current range</param>
		/// <param name="valueMax">upper bound of the value's current range</param>
		/// <param name="min">lower bound of the value's target range</param>
		/// <param name="max">upper bound of the value's target range</param>
		/// <returns>Equivalent of value between min and max</returns>
		public static float Map(float value, float valueMin, float valueMax, float min, float max)
		{
			return min + (value - valueMin) * (max - min) / (valueMax - valueMin);
		}

		/// <summary>
		/// 3 point lerp
		/// </summary>
		/// <param name="a">1st point</param>
		/// <param name="b">2nd point</param>
		/// <param name="c">3rd point</param>
		/// <param name="t">Value used to interpolate between points</param>
		public static Vector3 QuadraticLerp(Vector3 a, Vector3 b, Vector3 c, float t)
		{
			var ab = Vector3.Lerp(a, b, t);
			var bc = Vector3.Lerp(b, c, t);

			return Vector3.Lerp(ab, bc, t);
		}

		/// <summary>
		/// 4 point lerp
		/// </summary>
		/// <param name="a">1st point</param>
		/// <param name="b">2nd point</param>
		/// <param name="c">3rd point</param>
		/// <param name="d">4th point</param>
		/// <param name="t">Value used to interpolate between points</param>
		public static Vector3 CubicLerp(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
		{
			var ab_bc = QuadraticLerp(a, b, c, t);
			var bc_cd = QuadraticLerp(b, c, d, t);

			return Vector3.Lerp(ab_bc, bc_cd, t);
		}
	}
}