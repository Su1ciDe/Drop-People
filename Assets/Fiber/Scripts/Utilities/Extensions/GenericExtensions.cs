namespace Fiber.Utilities.Extensions
{
	public static class GenericExtensions
	{
		/// <summary>
		/// Returns a value indicating whether this instance is not equal to a specified value.
		/// </summary>
		/// <param name="other">A value to compare to this instance.</param>
		public static bool NotEquals<T>(this T obj, T other) => !obj.Equals(other);
	}
}