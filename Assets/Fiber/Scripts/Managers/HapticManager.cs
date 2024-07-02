using System.Collections;
using Fiber.Utilities;
using AYellowpaper.SerializedCollections;
using Lofelt.NiceVibrations;
using TriInspector;
using UnityEngine;

namespace Fiber.Managers
{
	public class HapticManager : SingletonInit<HapticManager>
	{
		[SerializeField] private bool hapticsEnabled = true;
		public bool HapticsEnabled
		{
			get => PlayerPrefs.GetInt(PlayerPrefsNames.HAPTICS_ENABLED, 1).Equals(1);
			set
			{
				PlayerPrefs.SetInt(PlayerPrefsNames.HAPTICS_ENABLED, value ? 1 : 0);
				HapticController.hapticsEnabled = value;
			}
		}

		public bool IsPlaying => HapticController.IsPlaying() || hapticMultiple is not null;

		[Title("Advanced Haptics")]
		[SerializeField] private SerializedDictionary<AdvancedHapticType, HapticClip> clips;

		[Title("Debug")]
		[SerializeField] private bool showDebug;

		private Coroutine hapticMultiple;

		protected override void Awake()
		{
			base.Awake();
			HapticsEnabled = hapticsEnabled;
		}

		/// <summary>
		/// Plays a set of predefined haptic patterns.</summary>
		/// <param name="hapticType">Preset type</param>
		public void PlayHaptic(HapticPatterns.PresetType hapticType)
		{
			HapticPatterns.PlayPreset(hapticType);

			ShowDebugLog($"<color=aqua>Haptic Preset triggered: <color=lime>{hapticType}</color></color>");
		}

		/// <summary>
		/// Plays a single emphasis point
		/// </summary>
		/// <param name="amplitude">The amplitude of haptic, from 0.0 to 1.0</param>
		/// <param name="frequency">The frequency of haptic, from 0.0 to 1.0</param>
		public void PlayHaptic(float amplitude, float frequency)
		{
			HapticPatterns.PlayEmphasis(amplitude, frequency);

			ShowDebugLog(
				$"<color=aqua>Haptic Emphasis triggered: <color=orange>Amplitude:</color><color=lime>{amplitude}</color>, <color=orange>Frequency:</color><color=lime>{frequency}</color></color>");
		}

		/// <summary>
		/// Plays a continuous haptic
		/// </summary>
		/// <param name="amplitude">The amplitude of haptic, from 0.0 to 1.0</param>
		/// <param name="frequency">The frequency of haptic, from 0.0 to 1.0</param>
		/// <param name="duration">Play duration in seconds</param>
		public void PlayHaptic(float amplitude, float frequency, float duration)
		{
			StopHaptics();

			HapticPatterns.PlayConstant(amplitude, frequency, duration);

			ShowDebugLog(
				$"<color=aqua>Haptic Constant triggered: <color=orange>Amplitude:<color=lime>{amplitude}</color>, Frequency:<color=lime>{frequency}</color>, for <color=lime>{duration}</color> seconds</color></color>");
		}

		/// <summary>
		/// Plays an advanced predefined haptic
		/// </summary>
		/// <param name="advancedHapticType"></param>
		public void PlayHaptic(AdvancedHapticType advancedHapticType)
		{
			StopHaptics();

			HapticController.Play(clips[advancedHapticType]);

			ShowDebugLog($"<color=aqua>Haptic Advanced Preset triggered: <color=lime>{advancedHapticType}</color></color>");
		}

		/// <summary>
		/// Plays a predefined haptic clip
		/// </summary>
		/// <param name="hapticClip">Haptic Clip</param>
		public void PlayHaptic(HapticClip hapticClip)
		{
			StopHaptics();

			HapticController.Play(hapticClip);

			ShowDebugLog($"<color=aqua>Haptic Clip triggered: <color=lime>{hapticClip.name}</color></color>");
		}

		/// <summary>
		/// Plays a multiple emphasis haptics at given amount of times and delay
		/// </summary>
		/// <param name="amplitude">The amplitude of haptic, from 0.0 to 1.0</param>
		/// <param name="frequency">The frequency of haptic, from 0.0 to 1.0</param>
		/// <param name="amount">How many times the haptics should play</param>
		/// <param name="delayInBetween">The time between haptics <br/><i>Note: Delays are unscaled time</i></param>
		public void PlayHapticMultiple(float amplitude, float frequency, int amount, int delayInBetween)
		{
			StopHaptics();

			hapticMultiple = StartCoroutine(HapticMultiple(amplitude, frequency, amount, delayInBetween));
		}

		private IEnumerator HapticMultiple(float amplitude, float frequency, int amount, int delayInBetween)
		{
			ShowDebugLog(
				$"<color=aqua>Haptic Multiple Emphasis started: <color=orange>Amplitude:<color=lime>{amplitude}</color>, <color=orange>Frequency:</color><color=lime>{frequency}</color>, delay in between <color=lime>{delayInBetween}</color>, for <color=lime>{amount}</color> times.</color></color>");

			var delay = new WaitForSecondsRealtime(delayInBetween);

			for (int i = 0; i < amount; i++)
			{
				PlayHaptic(amplitude, frequency);
				yield return delay;
			}

			hapticMultiple = null;

			ShowDebugLog("<color=aqua>Haptic Multiple Emphasis <color=red>finished</color>!</color>");
		}

		/// <summary>
		/// Plays a multiple emphasis haptics at given amount of times and delay
		/// </summary>
		/// <param name="hapticType">Preset type</param>
		/// <param name="amount">How many times the haptics should play</param>
		/// <param name="delayInBetween">The time between haptics <br/><i>Note: Delays are unscaled time</i></param>
		public void PlayHapticMultiple(HapticPatterns.PresetType hapticType, int amount, int delayInBetween)
		{
			StopHaptics();

			hapticMultiple = StartCoroutine(HapticMultiple(hapticType, amount, delayInBetween));
		}

		private IEnumerator HapticMultiple(HapticPatterns.PresetType hapticType, int amount, int delayInBetween)
		{
			ShowDebugLog(
				$"<color=aqua>Haptic Multiple Emphasis started: <color=orange><color=lime>{hapticType}</color>, delay in between <color=lime>{delayInBetween}</color>, for <color=lime>{amount}</color> times.</color></color>");

			var delay = new WaitForSecondsRealtime(delayInBetween);

			for (int i = 0; i < amount; i++)
			{
				PlayHaptic(hapticType);
				yield return delay;
			}

			hapticMultiple = null;

			ShowDebugLog("<color=aqua>Haptic Multiple Emphasis <color=red>finished</color>!</color>");
		}

		public void StopHaptics()
		{
			if (hapticMultiple is not null)
			{
				StopCoroutine(hapticMultiple);
				hapticMultiple = null;
			}

			if (HapticController.IsPlaying())
				HapticController.Stop();

			ShowDebugLog("<color=red>Stopped all haptics!</color>");
		}

		private void ShowDebugLog(string message)
		{
#if UNITY_EDITOR
			if (!showDebug) return;
			Debug.Log(message, gameObject);
#endif
		}

		public enum AdvancedHapticType
		{
			None = -1,
			Carillon,
			Dice,
			Drums,
			GameOver,
			Heartbeats,
			Laser,
			PowerOff,
			Reload,
			Teleport
		}
	}
}