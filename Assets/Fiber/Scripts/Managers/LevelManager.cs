using System.Collections.Generic;
using Fiber.Utilities;
using Fiber.AudioSystem;
using Fiber.LevelSystem;
using ScriptableObjects;
using TriInspector;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Fiber.Managers
{
	[DefaultExecutionOrder(-2)]
	public class LevelManager : Singleton<LevelManager>
	{
		/// <summary>
		/// Number of the level currently played. This value is not modulated.
		/// </summary>
		public int LevelNo
		{
			get => PlayerPrefs.GetInt(PlayerPrefsNames.LEVEL_NO, 1);
			set => PlayerPrefs.SetInt(PlayerPrefsNames.LEVEL_NO, value);
		}

		[Tooltip("Randomizes levels after all levels are played.\nIf this is unchecked, levels will be played again in same order.")]
		[SerializeField] private bool randomizeAfterRotation = true;

		[SerializeField] private Level levelPrefab;

		[Required]
		[ListDrawerSettings(Draggable = true, HideAddButton = false, HideRemoveButton = false, AlwaysExpanded = false)]
		public LevelDataSO[] Levels;
		[Tooltip("If you have tutorial levels add them here to extract them from rotation")]
		public LevelDataSO[] TutorialLevels;
		public LevelDataSO CurrentLevelData { get; private set; }
		public Level CurrentLevel { get; private set; }

		// Index of the level currently played
		private int currentLevelIndex;

		public static event UnityAction OnLevelLoad;
		public static event UnityAction OnLevelUnload;
		public static event UnityAction OnLevelStart;
		public static event UnityAction OnLevelWin;
		public static event UnityAction OnLevelLose;

		private void Awake()
		{
			if (Levels is null || Levels.Length.Equals(0))
			{
				Debug.LogWarning(name + ": There isn't any level added to the script!", this);
			}
		}

		private void Start()
		{
#if UNITY_EDITOR
			var levels = FindObjectsByType<Level>(FindObjectsInactive.Include, FindObjectsSortMode.None);
			foreach (var level in levels)
				level.gameObject.SetActive(false);
#endif
			LoadCurrentLevel();
		}

		public void LoadCurrentLevel()
		{
			int tutorialCount = TutorialLevels.Length;
			int levelCount;
			int levelIndex = LevelNo;

			if (LevelNo <= tutorialCount)
				levelCount = tutorialCount;
			else
			{
				levelCount = Levels.Length;
				levelIndex -= tutorialCount;
			}

			if (levelIndex <= levelCount)
			{
				currentLevelIndex = levelIndex;
			}
			else if (randomizeAfterRotation)
			{
				currentLevelIndex = Random.Range(1, levelCount + 1);
			}
			else
			{
				levelIndex %= levelCount;
				currentLevelIndex = levelIndex.Equals(0) ? levelCount : levelIndex;
			}

			LoadLevel(currentLevelIndex);
		}

		private void LoadLevel(int index)
		{
			CurrentLevel = Instantiate(levelPrefab, transform);
			CurrentLevelData = ScriptableObject.Instantiate(LevelNo <= TutorialLevels.Length ? TutorialLevels[index - 1] : Levels[index - 1]);
			CurrentLevel.Load(CurrentLevelData);
			OnLevelLoad?.Invoke();

			StartLevel();
		}

		public void StartLevel()
		{
			CurrentLevel.Play();
			OnLevelStart?.Invoke();
		}

		public void RetryLevel()
		{
			UnloadLevel();

			LoadLevel(currentLevelIndex);
		}

		public void LoadNextLevel()
		{
			UnloadLevel();

			LevelNo++;
			LoadCurrentLevel();
		}

		private void UnloadLevel()
		{
			OnLevelUnload?.Invoke();
			Destroy(CurrentLevel.gameObject);
		}

		public void Win()
		{
			if (StateManager.Instance.CurrentState != GameState.OnStart) return;

			AudioManager.Instance.PlayAudio(AudioName.LevelWin);
			OnLevelWin?.Invoke();
		}

		public void Lose()
		{
			if (StateManager.Instance.CurrentState != GameState.OnStart) return;

			AudioManager.Instance.PlayAudio(AudioName.LevelLose);
			OnLevelLose?.Invoke();
		}

#if UNITY_EDITOR
		[Button(ButtonSizes.Medium, "Add Level Assets To List")]
		private void AddLevelAssetsToList()
		{
			const string levelPath = "Assets/_Main/ScriptableObjects/Levels";
			var levels = EditorUtilities.LoadAllAssetsFromPath<LevelDataSO>(levelPath);
			var normalLevels = new List<LevelDataSO>();
			var tutorialLevels = new List<LevelDataSO>();

			foreach (var level in levels)
			{
				if (level.name.Contains("TEST")) continue;
				if (level.name.Contains("_Base")) continue;

				if (level.name.Contains("Tutorial"))
					tutorialLevels.Add(level);
				else
					normalLevels.Add(level);
			}

			Levels = normalLevels.ToArray();
			TutorialLevels = tutorialLevels.ToArray();
		}
#endif
	}
}