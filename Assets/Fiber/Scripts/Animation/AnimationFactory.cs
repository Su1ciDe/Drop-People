using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fiber.Animation
{
	/// <summary>
	/// This factory hashes all the animation parameters in an Enum to get better performance at runtime
	/// </summary>
	public static class AnimationFactory
	{
		private static Dictionary<int, int> hashedAnimations;
		private static bool isInitialized => hashedAnimations != null;

		public static void SetupAnimationHashes()
		{
			if (isInitialized) return;

			var names = Enum.GetNames(typeof(AnimationType));
			var values = Enum.GetValues(typeof(AnimationType));

			hashedAnimations = new Dictionary<int, int>();
			for (int i = 0; i < names.Length; i++)
				hashedAnimations.Add((int)(AnimationType)values.GetValue(i), Animator.StringToHash(names[i]));
		}

		/// <summary>
		/// Gets generated parameter id of given animation.
		/// <br/><b>Note:</b> You need to declare "AnimationType" enum beforehand
		/// </summary>
		/// <param name="animationType">The enum of the animation</param>
		/// <returns>Hashed animation parameter id</returns>
		internal static int GetAnimation(AnimationType animationType)
		{
			SetupAnimationHashes();
			if (hashedAnimations.TryGetValue((int)animationType, out var animHash))
				return animHash;
			else
			{
#if UNITY_EDITOR
				Debug.LogWarning("There's no hashed animation by this name: " + animationType);
#endif
				return -1;
			}
		}
	}
}