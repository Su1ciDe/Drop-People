namespace Fiber.Utilities.Extensions
{
	public static class NumberExtensions
	{
		#region Float

		/// <summary>
		/// Is a float value between 0 and 1. 0 and 1 is exclusive
		/// </summary>
		/// <returns>Is between 0 - 1 or not</returns>
		public static bool Is01(this float value)
		{
			return value is > 0f and < 1f;
		}

		/// <summary>
		/// Returns a value indicating whether this instance is not equal to a specified float.
		/// </summary>
		/// <param name="other">A float to compare to this instance.</param>
		public static bool NotEquals(this float number, int other) => !number.Equals(other);

		#endregion

		#region Int

		/// <summary>
		/// Returns a value indicating whether this instance is not equal to a specified int.
		/// </summary>
		/// <param name="other">An int to compare to this instance.</param>
		public static bool NotEquals(this int number, int other) => !number.Equals(other);

		#endregion

		#region Double

		/// <summary>
		/// Returns a value indicating whether this instance is not equal to a specified double.
		/// </summary>
		/// <param name="other">A double to compare to this instance.</param>
		public static bool NotEquals(this double number, double other) => !number.Equals(other);

		#endregion
	}
}