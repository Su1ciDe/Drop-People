using Fiber.Managers;
using Fiber.Utilities;
using UnityEngine.Events;

namespace Managers
{
	public class StageManager : Singleton<StageManager>
	{
		public int CurrentStageIndex { get; private set; }
		public int StageCount { get; private set; }

		public static event UnityAction OnStageInit;
		public static event UnityAction<int> OnStageStarted;
		public static event UnityAction<int> OnStageCompleted;

		private void OnEnable()
		{
			LevelManager.OnLevelLoad += OnLevelLoaded;
			LevelManager.OnLevelStart += OnLevelStarted;
		}

		private void OnDisable()
		{
			LevelManager.OnLevelLoad -= OnLevelLoaded;
			LevelManager.OnLevelStart -= OnLevelStarted;
		}

		private void OnLevelLoaded()
		{
			StageCount = LevelManager.Instance.CurrentLevelData.GoalStages.Length;
			CurrentStageIndex = 0;
			
			OnStageInit?.Invoke();
		}

		private void OnLevelStarted()
		{
			StartStage();
		}

		private void StartStage()
		{
			OnStageStarted?.Invoke(CurrentStageIndex);
		}

		public void StageComplete()
		{
			if (CurrentStageIndex + 1 >= StageCount)
			{
				LevelManager.Instance.Win();
			}
			else
			{
				OnStageCompleted?.Invoke(CurrentStageIndex);
				CurrentStageIndex++;
			}
		}

		public void StartNextStage()
		{
			OnStageStarted?.Invoke(CurrentStageIndex);
		}
	}
}