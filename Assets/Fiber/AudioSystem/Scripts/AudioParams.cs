using UnityEngine;

namespace Fiber.AudioSystem
{
	public class AudioParams
	{
		// public static readonly AudioParams Params = new AudioParams();

		public float Volume = 1f;

		public float FadeDuration = 0f;
		public WaitForSeconds Delay;

		public float Length;

		public bool IsRandomPitch = false;
		public float MinPitch = 0f;
		public float MaxPitch = 0f;

		public float Pitch = 1f;

		public bool Looped = false;

		public bool IsSpatialBlend = false;
		public float SpatialBlend = 0;

		public bool PlayOnGamepad = false;
		public int GamepadIndex = 0;
	}
}