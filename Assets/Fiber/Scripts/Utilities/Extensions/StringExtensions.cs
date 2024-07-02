namespace Fiber.Utilities.Extensions
{
	public static class StringExtensions
	{
		/// <summary>
		/// Returns a value indicating whether this instance is not equal to a specified string.
		/// </summary>
		/// <param name="other">A string to compare to this instance.</param>
		public static bool NotEquals(this string str, string other) => !str.Equals(other);
		
		/// <summary>
		/// Returns a value indicating whether this instance is not equal to a specified char.
		/// </summary>
		/// <param name="other">A char to compare to this instance.</param>
		public static bool NotEquals(this char character, char other) => !character.Equals(other);
	}
}