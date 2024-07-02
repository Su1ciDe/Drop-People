using UnityEngine;

namespace Fiber.AudioSystem
{
	public static class AudioExtensions
	{
		public static AudioJob SetVolume(this AudioJob job, float volume)
		{
			job.Params.Volume = volume;
			return job;
		}

		public static AudioJob SetFade(this AudioJob job, float fadeDuration)
		{
			job.Params.FadeDuration = fadeDuration;
			return job;
		}

		public static AudioJob SetDelay(this AudioJob job, float delay)
		{
			job.Params.Delay = new WaitForSeconds(delay);
			return job;
		}

		public static AudioJob SetLoop(this AudioJob job, bool looped)
		{
			job.Params.Looped = looped;
			return job;
		}

		public static AudioJob SetLength(this AudioJob job, float length)
		{
			job.Params.Length = length;
			return job;
		}

		public static AudioJob SetPitch(this AudioJob job, float pitch)
		{
			job.Params.Pitch = pitch;
			return job;
		}

		public static AudioJob SetRandomPitch(this AudioJob job, float minPitch, float maxPitch)
		{
			job.Params.IsRandomPitch = true;
			job.Params.MinPitch = minPitch;
			job.Params.MaxPitch = maxPitch;

			return job;
		}

		public static AudioJob SetSpatialBlend(this AudioJob job, float spatialBlend)
		{
			job.Params.IsSpatialBlend = true;
			job.Params.SpatialBlend = spatialBlend;

			return job;
		}
	}
}