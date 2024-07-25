using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Fiber.Managers;
using Fiber.Utilities;
using GamePlay.People;
using GoalSystem;
using PathCreation;
using TriInspector;
using UnityEngine;
using UnityEngine.Events;
using Grid = GridSystem.Grid;

namespace Managers
{
	[DeclareHorizontalGroup("lines")]
	public class GoalManager : Singleton<GoalManager>
	{
		public bool IsGoalSequence { get; private set; }
		public List<GoalHolder> CurrentGoalHolders { get; private set; } = new List<GoalHolder>(LINE_COUNT);

		[SerializeField] private GoalHolder goalHolderPrefab;
		[SerializeField] private float goalHolderLength;
		[Title("Lines")]
		[Group("lines")] [SerializeField] private Transform[] lines = new Transform[LINE_COUNT];
		[Title("Lines")]
		[Group("lines")] [SerializeField] private PathCreator[] linePaths = new PathCreator[LINE_COUNT];

		private List<Queue<GoalHolder>> lineQueues = new List<Queue<GoalHolder>>();
		// private Dictionary<int, List<GoalHolder>> holders = new Dictionary<int, List<GoalHolder>>();

		public const float DELAY = .2F;
		public const int LINE_COUNT = 3;

		public static event UnityAction OnGoal;
		public static event UnityAction<GoalHolder> OnNewGoal;

		private void OnEnable()
		{
			LevelManager.OnLevelLoad += OnLevelLoaded;
			LevelManager.OnLevelStart += OnLevelStarted;
			PersonGroup.OnComplete += OnPersonGroupCompleted;
		}

		private void OnDisable()
		{
			LevelManager.OnLevelLoad -= OnLevelLoaded;
			LevelManager.OnLevelStart -= OnLevelStarted;
			PersonGroup.OnComplete -= OnPersonGroupCompleted;
		}

		private void OnLevelLoaded()
		{
			lineQueues.Add(new Queue<GoalHolder>());
			lineQueues.Add(new Queue<GoalHolder>());
			lineQueues.Add(new Queue<GoalHolder>());

			for (var i = 0; i < LevelManager.Instance.CurrentLevelData.GoalStages.Length; i++)
			{
				var goalStage = LevelManager.Instance.CurrentLevelData.GoalStages[i];
				for (int j = 0; j < LINE_COUNT; j++)
				{
					if (goalStage.Goals.Length <= j) continue;

					var line = lines[j];
					var goal = goalStage.Goals[j];

					var goalHolder = Instantiate(goalHolderPrefab, line.transform);
					goalHolder.transform.localPosition = new Vector3(-goalHolderLength * i, 0, 0);
					goalHolder.Setup(goal.GoalColor.PersonType, goal.Count, j);
					goalHolder.OnComplete += OnGoalCompleted;

					lineQueues[j].Enqueue(goalHolder);
				}
			}
		}

		private void OnGoalCompleted(GoalHolder goalHolder)
		{
			goalHolder.OnComplete -= OnGoalCompleted;

			var index = goalHolder.LineIndex;

			StartCoroutine(MoveToEndOnComplete(goalHolder, index));

			if (!lineQueues[index].TryDequeue(out var nextGoalHolder)) return;

			CurrentGoalHolders[index] = nextGoalHolder;
			nextGoalHolder.MoveTo(lines[index].position).SetDelay(0.2f).OnComplete(() => OnNewGoal?.Invoke(nextGoalHolder));
			nextGoalHolder.OnCurrentGoal();

			int i = 1;
			foreach (var holder in lineQueues[index])
			{
				holder.MoveTo(lines[index].position + i * goalHolderLength * Vector3.forward).SetDelay(0.2f);
				i++;
			}
		}

		private IEnumerator MoveToEndOnComplete(GoalHolder goalHolder, int index)
		{
			int count = 0;

			foreach (var lineQueue in lineQueues)
			{
				foreach (var holder in lineQueue)
				{
					if (holder && !holder.IsCompleted)
						count++;
				}
			}

			for (int j = 0; j < CurrentGoalHolders.Count; j++)
			{
				if (CurrentGoalHolders[j] && !CurrentGoalHolders[j].IsCompleted)
					count++;
			}

			yield return new WaitForSeconds(0.2f);
			yield return StartCoroutine(goalHolder.MoveToEndCoroutine(linePaths[index].path));

			if (count <= 0)
			{
				if (levelCompleteCoroutine is not null)
				{
					StopCoroutine(levelCompleteCoroutine);
					levelCompleteCoroutine = null;
				}

				levelCompleteCoroutine = StartCoroutine(LevelCompleteCoroutine());
			}
		}

		private Coroutine levelCompleteCoroutine;

		private IEnumerator LevelCompleteCoroutine()
		{
			yield return new WaitForSeconds(1);
			yield return new WaitUntil(() => !IsGoalSequence);

			LevelManager.Instance.Win();
		}

		private void OnLevelStarted()
		{
			CurrentGoalHolders.Clear();

			for (int i = 0; i < LINE_COUNT; i++)
			{
				if (!lineQueues[i].TryDequeue(out var goalHolder)) continue;

				CurrentGoalHolders.Add(goalHolder);
				goalHolder.OnCurrentGoal();
			}
		}

		public void OnPersonGroupCompleted(PersonGroup personGroup)
		{
			var personType = personGroup.GetPersonTypes()[0];
			var goalHolder = GetCurrentGoalHolder(personType);
			if (!goalHolder)
			{
				personGroup.CloseCover();
				return;
			}

			if (goalHolder.IsCompleted) return;

			Grid.Instance.CheckObstacles(personGroup);

			StartCoroutine(GroupCompleteCoroutine(personGroup, goalHolder));
		}

		private IEnumerator GroupCompleteCoroutine(PersonGroup personGroup, GoalHolder goalHolder)
		{
			IsGoalSequence = true;

			yield return StartCoroutine(goalHolder.SetPeople(personGroup));
			
			IsGoalSequence = false;
			OnGoal?.Invoke();
		}

		public GoalHolder GetCurrentGoalHolder(PersonType personType)
		{
			for (var i = 0; i < CurrentGoalHolders.Count; i++)
			{
				var currentGoalHolder = CurrentGoalHolders[i];
				if (!currentGoalHolder) continue;
				if (currentGoalHolder.IsCompleted) continue;
				if (currentGoalHolder.PersonType == personType)
					return currentGoalHolder;
			}

			return null;
		}
	}
}