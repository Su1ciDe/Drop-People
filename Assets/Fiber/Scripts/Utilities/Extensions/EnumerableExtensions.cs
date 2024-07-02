using System;
using System.Linq;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Fiber.Utilities.Extensions
{
	public static class EnumerableExtensions
	{
		#region Array

		/// <summary>
		/// Adds an item to the end of a new array
		/// </summary>
		/// <param name="item">Item to be added</param>
		/// <returns>Creates a new array of your array and added item</returns>
		public static T[] Add<T>(this T[] array, T item)
		{
			Array.Resize(ref array, array.Length + 1);
			array[^1] = item;
			return array;
		}

		/// <summary>
		/// Adds an array to the end of a new array
		/// </summary>
		/// <param name="collection">Array to be added</param>
		/// <returns>Creates a new array of your array and added array</returns>
		public static T[] AddRange<T>(this T[] array, T[] collection)
		{
			int startingSize = array.Length;
			int newSize = startingSize + collection.Length;
			Array.Resize(ref array, newSize);
			Array.Copy(collection, 0, array, startingSize, collection.Length);
			return array;
		}

		/// <summary>
		/// Picks a random item from the array.
		/// </summary>
		/// <returns>A random item</returns>
		public static T RandomItem<T>(this T[] array)
		{
			if (array.Length.Equals(0))
				return default;

			var rnd = new System.Random(Random.Range(0, int.MaxValue));
			var index = rnd.Next(0, array.Length);
			return array[index];
		}

		/// <summary>
		/// Picks a random item from the array according to probability table
		/// </summary>
		/// <param name="array">Your array</param>
		/// <param name="weights">Weights of the array items corresponding to items index</param>
		/// <returns>A random item</returns>
		public static T WeightedRandom<T>(this T[] array, int[] weights)
		{
			int totalPriority = 0;
			int itemCount = array.Length;
			var weightsOriginal = new List<int>(weights);

			for (int i = 0; i < itemCount; i++)
			{
				weightsOriginal[i] += totalPriority;
				totalPriority += weights[i];
			}

			int randomPriority = Random.Range(0, totalPriority);

			for (int i = 0; i < itemCount; i++)
			{
				if (weightsOriginal[i] > randomPriority)
					return array[i];
			}

			return array[0];
		}

		/// <summary>
		/// Shuffles the array
		/// </summary>
		public static void Shuffle<T>(this T[] array)
		{
			var rng = new System.Random(Random.Range(0, int.MaxValue));
			var n = array.Length;
			while (n > 1)
			{
				n--;
				var k = rng.Next(n + 1);
				(array[k], array[n]) = (array[n], array[k]);
			}
		}

		/// <summary>
		/// Returns a value indicating whether this instance is not equal to a specified array.
		/// </summary>
		/// <param name="other">An array to compare to this instance.</param>
		public static bool NotEquals<T>(this T[] array, T[] other) => !array.Equals(other);

		#endregion

		#region List
		
		/// <summary>
		/// Adds an item to the current collection if it is not already in the collection
		/// </summary>
		/// <param name="item">The item to add to current collection</param>
		/// <typeparam name="T"></typeparam>
		public static void AddIfNotContains<T>(this IList<T> list, T item)
		{
			if (!list.Contains(item))
				list.Add(item);
		}

		/// <summary>
		/// Picks a random item from the list.
		/// </summary>
		/// <returns>A random item</returns>
		public static T RandomItem<T>(this IList<T> list)
		{
			if (list.Count.Equals(0))
				return default;

			var rnd = new System.Random(Random.Range(0, int.MaxValue));
			var index = rnd.Next(0, list.Count);
			return list[index];
		}

		/// <summary>
		/// Picks a random item and removes it from the list.
		/// </summary>
		/// <returns>A random item</returns>
		public static T PickRandomItem<T>(this List<T> list)
		{
			if (list.Count.Equals(0))
				return default;

			int pickIndex = Random.Range(0, list.Count);
			var picked = list[pickIndex];
			list.Remove(picked);
			return picked;
		}

		/// <summary>
		/// Picks a random item from the list according to probability table
		/// </summary>
		/// <param name="list">Your list</param>
		/// <param name="weights">Weights of the list items corresponding to items index</param>
		/// <returns>A random item</returns>
		public static T WeightedRandom<T>(this List<T> list, List<int> weights)
		{
			int itemCount = list.Count;
			var weightsOriginal = new List<int>(weights);
			int totalPriority = WeightedRandom(ref weightsOriginal, weights, itemCount);

			int randomPriority = Random.Range(0, totalPriority);

			for (int i = 0; i < itemCount; i++)
			{
				if (weightsOriginal[i] > randomPriority)
					return list[i];
			}

			return list[0];
		}

		/// <summary>
		/// Picks a random item from the list according to probability table and removes it from the list
		/// </summary>
		/// <param name="list">Your list</param>
		/// <param name="weights">Weights of the list items corresponding to items index</param>
		/// <returns>A random item</returns>
		public static T PickWeightedRandom<T>(this List<T> list, ref List<int> weights)
		{
			int itemCount = list.Count;
			var weightsOriginal = new List<int>(weights);
			var totalPriority = WeightedRandom(ref weightsOriginal, weights, itemCount);

			int randomPriority = Random.Range(0, totalPriority);

			for (int i = 0; i < itemCount; i++)
			{
				if (weightsOriginal[i] > randomPriority)
				{
					var item = list[i];
					list.RemoveAt(i);
					weights.RemoveAt(i);
					return item;
				}
			}

			var item0 = list[0];
			list.RemoveAt(0);
			weights.RemoveAt(0);
			return item0;
		}

		private static int WeightedRandom(ref List<int> weightsOriginal, IReadOnlyList<int> weights, int itemCount)
		{
			int totalPriority = 0;
			for (int i = 0; i < itemCount; i++)
			{
				weightsOriginal[i] += totalPriority;
				totalPriority += weights[i];
			}

			return totalPriority;
		}

		/// <summary>
		/// Shuffles the list
		/// </summary>
		public static void Shuffle<T>(this IList<T> list)
		{
			var rng = new System.Random(Random.Range(0, int.MaxValue));
			var n = list.Count;
			while (n > 1)
			{
				n--;
				var k = rng.Next(n + 1);
				(list[k], list[n]) = (list[n], list[k]);
			}
		}

		/// <summary>
		/// Returns a value indicating whether this instance is not equal to a specified list.
		/// </summary>
		/// <param name="other">A list to compare to this instance.</param>
		public static bool NotEquals<T>(this List<T> list, List<T> other) => !list.Equals(other);

		#endregion

		#region Dictionary

		/// <summary>
		/// Picks a random item from the dictionary.
		/// </summary>
		/// <returns>A random value</returns>
		public static TValue RandomValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
		{
			var rand = new System.Random();
			var values = dictionary.Values.ToList();
			int size = dictionary.Count;
			return values[rand.Next(size)];
		}

		/// <summary>
		/// Picks a unique random item and removes it from the dictionary.
		/// </summary>
		/// <returns>A unique random value</returns>
		public static TValue PickRandomValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
		{
			var rand = new System.Random();
			var values = new Dictionary<TKey, TValue>(dictionary);

			var randomKey = values.Keys.ElementAt(rand.Next(0, values.Count));
			var randomValue = values[randomKey];
			dictionary.Remove(randomKey);
			return randomValue;
		}

		/// <summary>
		/// Returns a value indicating whether this instance is not equal to a specified dictionary.
		/// </summary>
		/// <param name="other">A dictionary to compare to this instance.</param>
		public static bool NotEquals<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, Dictionary<TKey, TValue> other) => !dictionary.Equals(other);

		#endregion

		#region Enum

		public static int GetCount<T>(this T enumType) where T : Enum
		{
			return Enum.GetNames(typeof(T)).Length;
		}

		public static IEnumerable<T> ToEnumerable<T>(this T myEnum, params T[] exclusions) where T : Enum
		{
			var values = Enum.GetValues(typeof(T));
			for (int i = 0; i < values.Length; i++)
			{
				var value = (T)values.GetValue(i);
				if (!exclusions.Contains(value))
					yield return value;
			}
		}

		public static List<T> ToList<T>(this T myEnum, params T[] exclusions) where T : Enum
		{
			return myEnum.ToEnumerable(exclusions).ToList();
		}

		public static T RandomItem<T>(this T myEnum) where T : Enum
		{
			var values = Enum.GetValues(typeof(T));
			var r = Random.Range(0, values.Length);
			return (T)values.GetValue(r);
		}

		public static IEnumerable<int> GetFlags<T>(T flag) where T : Enum
		{
			var enums = Enum.GetValues(typeof(T));
			for (int i = 0; i < enums.Length; i++)
			{
				//var layer = 1 << i;
				if (flag.HasFlag((T)enums.GetValue(i)))
					yield return i;
			}
		}

		#endregion
	}
}