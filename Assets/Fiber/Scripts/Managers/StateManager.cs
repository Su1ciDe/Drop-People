using System.Collections;
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

		private double completionTime;

		private const string PARAM_TIME = "time";
		private const string PARAM_MOVE_COUNT = "used_move_count";

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

			levelCompleteTimeCoroutine = StartCoroutine(LevelCompleteTime());
		}

		private void WinLevel()
		{
			Debug.Log("GAME WIN");

			StopCoroutine(levelCompleteTimeCoroutine);
			var param = Params.New().Set(PARAM_TIME, completionTime);
			Elephant.LevelCompleted(LevelManager.Instance.LevelNo, param);

			CurrentState = GameState.OnWin;

			completionTime = 0d;
		}

		private void WinLevelWithMoveCount(int moveCount)
		{
			Debug.Log("GAME WIN");

			StopCoroutine(levelCompleteTimeCoroutine);
			var param = Params.New().Set(PARAM_TIME, completionTime).Set(PARAM_MOVE_COUNT, moveCount);
			Elephant.LevelCompleted(LevelManager.Instance.LevelNo, param);

			CurrentState = GameState.OnWin;

			completionTime = 0d;
		}

		private void LoseLevel()
		{
			Debug.Log("GAME LOSE");

			Elephant.LevelFailed(LevelManager.Instance.LevelNo);

			CurrentState = GameState.OnLose;

			StopCoroutine(levelCompleteTimeCoroutine);
			completionTime = 0d;
		}

		private readonly WaitForSeconds wait = new WaitForSeconds(1);
		private Coroutine levelCompleteTimeCoroutine;

		private IEnumerator LevelCompleteTime()
		{
			while (Application.isFocused)
			{
				yield return wait;
				completionTime++;
			}
		}
	}
}