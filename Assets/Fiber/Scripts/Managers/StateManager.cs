using ElephantSDK;
using Fiber.Utilities;
using Fiber.LevelSystem;
using UnityEngine;
using UnityEngine.Events;

namespace Fiber.Managers
{
	public class StateManager : SingletonInit<StateManager>
	{
		public GameState CurrentState
		{
			get => gameState;
			set
			{
				gameState = value;
				OnStateChanged?.Invoke(gameState);
			}
		}

		[Header("Debug")]
		[SerializeField] private GameState gameState = GameState.None;

		public static event UnityAction<GameState> OnStateChanged;

		private void OnEnable()
		{
			LevelManager.OnLevelLoad += LevelLoading;
			LevelManager.OnLevelStart += StartLevel;
			LevelManager.OnLevelLose += LoseLevel;
			LevelManager.OnLevelWin += WinLevel;
		}

		private void OnDisable()
		{
			LevelManager.OnLevelLoad -= LevelLoading;
			LevelManager.OnLevelStart -= StartLevel;
			LevelManager.OnLevelLose -= LoseLevel;
			LevelManager.OnLevelWin -= WinLevel;
		}

		private void LevelLoading()
		{
			CurrentState = GameState.Loading;
		}

		private void StartLevel()
		{
			Debug.Log("GAME START");

			Elephant.LevelStarted(LevelManager.Instance.LevelNo);

			CurrentState = GameState.OnStart;
		}

		private void WinLevel()
		{
			Debug.Log("GAME WIN");

			Elephant.LevelCompleted(LevelManager.Instance.LevelNo);

			CurrentState = GameState.OnWin;
		}

		private void LoseLevel()
		{
			Debug.Log("GAME LOSE");

			Elephant.LevelFailed(LevelManager.Instance.LevelNo);

			CurrentState = GameState.OnLose;
		}
	}
}