using System.Linq;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using DG.Tweening;
using Fiber.Utilities;
using TriInspector;
using UnityEngine;
using UnityEngine.Pool;

namespace Fiber.AudioSystem
{
	public class AudioManager : Singleton<AudioManager>
	{
		public bool IsSoundMuted
		{
			get => PlayerPrefs.GetInt(PlayerPrefsNames.IS_SOUND_MUTED, 0).Equals(1);
			set => PlayerPrefs.SetInt(PlayerPrefsNames.IS_SOUND_MUTED, value ? 1 : 0);
		}
		public bool IsMusicMuted
		{
			get => PlayerPrefs.GetInt(PlayerPrefsNames.IS_MUSIC_MUTED, 0).Equals(1);
			set => PlayerPrefs.SetInt(PlayerPrefsNames.IS_MUSIC_MUTED, value ? 1 : 0);
		}

		[SerializeField] private int minPlayingAudioAtATime = 25;
		[SerializeField] private int maxPlayingAudioAtATime = 50;

		[System.Serializable]
		public class AudioTrack
		{
			[AssetsOnly]
			[Required]
			public AudioSource AudioSourcePrefab;
			public ObjectPool<AudioSource> AudioSourcePool;
			[HideInInspector] public AudioSource ActiveAudioSource;

			public bool CanPlayOver;
			[SerializedDictionary("Audio Name", "Audio Clip")]
			public SerializedDictionary<AudioName, AudioClip> Audios;
		}

		[Space]
		[SerializeField] private AudioTrack[] tracks;

		private Dictionary<AudioName, Coroutine> jobTable;
		private Dictionary<AudioName, AudioTrack> audioTable;

		private readonly List<AudioSource> currentPlayingAudioSources = new List<AudioSource>();

		private const string VOLUME_PARAM = "_Volume";

		private void Awake()
		{
			DontDestroyOnLoad(gameObject);

			jobTable = new Dictionary<AudioName, Coroutine>();
			audioTable = new Dictionary<AudioName, AudioTrack>();
			for (int i = 0; i < tracks.Length; i++)
			{
				var track = tracks[i];
				if (track.CanPlayOver)
				{
					track.AudioSourcePool = new ObjectPool<AudioSource>(() => Instantiate(track.AudioSourcePrefab, transform), source => source.gameObject.SetActive(true),
						source => source.gameObject.SetActive(false), source => Destroy(source.gameObject), false, minPlayingAudioAtATime, maxPlayingAudioAtATime);
				}
			}

			GenerateAudioTable();
		}

		private void Start()
		{
			Init();
		}

		private void Init()
		{
			ToggleAudioTrack(0, IsSoundMuted);
			ToggleAudioTrack(1, IsMusicMuted);
		}

		private void OnDisable()
		{
			Dispose();
		}

		/// <summary>
		/// Plays an audio. Also can add filters.
		/// </summary>
		/// <param name="audioName">Enum of the audio you want to play</param>
		public AudioJob PlayAudio(AudioName audioName)
		{
			var job = new AudioJob(AudioAction.Start, audioName);
			AddJob(job);
			return job;
		}

		/// <summary>
		/// Plays an audio at given position. Also can add filters.
		/// </summary>
		/// <param name="audioName">Enum of the audio you want to play</param>
		/// <param name="audioPosition">Position of the audio</param>
		public AudioJob PlayAudio(AudioName audioName, Vector3 audioPosition)
		{
			var job = new AudioJob(AudioAction.Start, audioName);
			AddJob(job, audioPosition);
			return job;
		}

		/// <summary>
		/// Stops the playing audio 
		/// </summary>
		/// <param name="audioName"></param>
		public void StopAudio(AudioName audioName)
		{
			var job = new AudioJob(AudioAction.Stop, audioName);
			AddJob(job);
		}

		/// <summary>
		/// Plays an audio at given position. Also can add filters.
		/// </summary>
		/// <param name="audioName">Enum of the audio you want to play</param>
		public AudioJob ReplayAudio(AudioName audioName)
		{
			var job = new AudioJob(AudioAction.Replay, audioName);
			AddJob(job);
			return job;
		}

		/// <summary>
		/// Replays an audio at given position. Also can add filters.
		/// </summary>
		/// <param name="audioName">Enum of the audio you want to play</param>
		/// <param name="audioPosition">Position of the audio</param>
		public AudioJob ReplayAudio(AudioName audioName, Vector3 audioPosition)
		{
			var job = new AudioJob(AudioAction.Replay, audioName);
			AddJob(job, audioPosition);
			return job;
		}

		public void ToggleAudioTrack(int index, bool mute)
		{
			var mixer = tracks[index].AudioSourcePrefab.outputAudioMixerGroup;
			if (mute)
				mixer.audioMixer.SetFloat(VOLUME_PARAM, -80);
			else
				mixer.audioMixer.SetFloat(VOLUME_PARAM, -10);
		}

		public bool GetIsMuted(int index)
		{
			var mixer = tracks[index].AudioSourcePrefab.outputAudioMixerGroup;
			mixer.audioMixer.GetFloat(VOLUME_PARAM, out var value);

			return value.Equals(-80);
		}

		#region Audio Logic

		private void AddJob(AudioJob job, Vector3 audioPosition = default)
		{
			var track = GetAudioTrack(job.AudioName); // track existence should be verified by now
			if (track is null) return;

			// cancel any job that might be using this job's audio source
			if (!track.CanPlayOver)
				RemoveConflictingJobs(job.AudioName);

			var jobRunner = StartCoroutine(RunAudioJob(track, job, audioPosition));
			jobTable.TryAdd(job.AudioName, jobRunner);
		}

		private IEnumerator RunAudioJob(AudioTrack track, AudioJob job, Vector3 audioPosition = default)
		{
			yield return null;
			if (job.Params.Delay is not null) yield return job.Params.Delay;

			// If it can play over get source from pool and play the clip
			AudioSource source;
			if (track.CanPlayOver)
			{
				source = track.AudioSourcePool.Get();
				source.clip = GetAudioClipFromAudioTrack(job.AudioName, track);

				DOVirtual.DelayedCall(job.Params.Length.Equals(0) ? source.clip.length : job.Params.Length, () =>
				{
					track.AudioSourcePool.Release(source);
					currentPlayingAudioSources.Remove(source);
				});
			}
			else
			{
				if (!track.ActiveAudioSource)
					track.ActiveAudioSource = Instantiate(track.AudioSourcePrefab, transform);
				track.ActiveAudioSource.clip = GetAudioClipFromAudioTrack(job.AudioName, track);
				source = track.ActiveAudioSource;
			}

			DoJob(source, track, job, audioPosition);

			jobTable.Remove(job.AudioName);
		}

		private void DoJob(AudioSource source, AudioTrack track, AudioJob job, Vector3 position)
		{
			// Volume 
			source.volume = job.Params.Volume;

			// Audio Position, make it 3D sound
			if (!position.Equals(default))
			{
				source.transform.position = position;
				source.spatialBlend = 1;
			}
			else
				source.spatialBlend = 0;

			// Spatial Blend
			if (job.Params.IsSpatialBlend)
				source.spatialBlend = job.Params.SpatialBlend;

			// Loop
			source.loop = job.Params.Looped;

			// Pitch
			if (job.Params.IsRandomPitch)
				source.pitch = Random.Range(job.Params.MinPitch, job.Params.MaxPitch);
			else if (!job.Params.Pitch.Equals(0))
				source.pitch = job.Params.Pitch;
			else
				source.pitch = 1;

			float target = 1f;

			switch (job.AudioAction)
			{
				case AudioAction.Start:
					target = job.Params.Volume;
					currentPlayingAudioSources.Add(source);
					source.Play();
					break;
				case AudioAction.Stop when job.Params.FadeDuration.Equals(0):
					if (track.CanPlayOver)
					{
						var audioTrack = currentPlayingAudioSources.FirstOrDefault(x => x.clip.Equals(source.clip));
						if (audioTrack)
						{
							audioTrack.Stop();
							currentPlayingAudioSources.Remove(audioTrack);
						}
					}
					else
					{
						source.Stop();
						currentPlayingAudioSources.Remove(source);
					}

					break;
				case AudioAction.Stop:
					currentPlayingAudioSources.Remove(source);
					target = 0f;
					break;
				case AudioAction.Replay:
					source.Stop();
					source.Play();
					break;
			}

			// Fade volume
			if (!job.Params.FadeDuration.Equals(0))
			{
				source.DOComplete();
				source.volume = 0;
				source.DOFade(target, job.Params.FadeDuration).SetEase(Ease.Linear).OnComplete(() =>
				{
					if (job.AudioAction == AudioAction.Stop)
					{
						track.ActiveAudioSource.Stop();
					}
				});
			}
		}

		private void RemoveJob(AudioName audioName)
		{
			if (!jobTable.ContainsKey(audioName)) return;

			var runningJob = jobTable[audioName];
			StopCoroutine(runningJob);
			jobTable.Remove(audioName);
		}

		private void RemoveConflictingJobs(AudioName audioName)
		{
			// cancel the job if one exists with the same type
			if (jobTable.ContainsKey(audioName))
				RemoveJob(audioName);

			// cancel jobs that share the same audio track
			var conflictAudio = AudioName.None;
			var audioTrackNeeded = GetAudioTrack(audioName);

			if (audioTrackNeeded is not null)
			{
				foreach (var jobAudioName in jobTable.Keys)
				{
					var audioTrackInUse = GetAudioTrack(jobAudioName);
					if (audioTrackInUse.ActiveAudioSource && audioTrackNeeded.ActiveAudioSource && audioTrackInUse.ActiveAudioSource.Equals(audioTrackNeeded.ActiveAudioSource))
					{
						conflictAudio = jobAudioName;
						break;
					}
				}
			}

			if (conflictAudio != AudioName.None)
			{
				RemoveJob(conflictAudio);
			}
		}

		private void GenerateAudioTable()
		{
			foreach (var track in tracks)
			{
				foreach (var audioName in track.Audios.Keys)
					audioTable.TryAdd(audioName, track);
			}
		}

		private AudioTrack GetAudioTrack(AudioName audioName)
		{
			return !audioTable.TryGetValue(audioName, out var track) ? null : track;
		}

		private AudioClip GetAudioClipFromAudioTrack(AudioName audioName, AudioTrack track)
		{
			foreach (var clip in track.Audios)
				if (clip.Key == audioName)
					return clip.Value;

			return null;
		}

		private void Dispose()
		{
			foreach (var job in jobTable)
			{
				var jobCoroutine = job.Value;
				StopCoroutine(jobCoroutine);
			}

			currentPlayingAudioSources.Clear();
		}

		#endregion
	}
}