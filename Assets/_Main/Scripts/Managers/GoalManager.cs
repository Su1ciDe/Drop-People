using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Fiber.AudioSystem;
using Fiber.Managers;
using Fiber.Utilities;
using GamePlay.People;
using GoalSystem;
using TriInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Managers
{
	public class GoalManager : Singleton<GoalManager>
	{
		public bool IsGoalSequence { get; set; } = false;
		// public GoalHolder CurrentGoalHolder { get; private set; }
		public List<GoalHolder> CurrentGoalHolders { get; private set; } = new List<GoalHolder>(LINE_COUNT);

		[SerializeField] private GoalHolder goalHolderPrefab;
		[SerializeField] private float goalHolderLength;
		[Title("Points")]
		[SerializeField] private Transform goalHolderPoint;
		[SerializeField] private Transform goalHolderNextPoint;
		[SerializeField] private Transform goalHolderMovePoint;
		[Title("Lines")]
		[SerializeField] private Transform[] lines = new Transform[LINE_COUNT];

		private List<Queue<GoalHolder>> lineQueues = new List<Queue<GoalHolder>>();
		// private Dictionary<int, List<GoalHolder>> holders = new Dictionary<int, List<GoalHolder>>();

		public const float DELAY = .05F;
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
					var line = lines[j];
					var goal = goalStage.Goals[j];

					var goalHolder = Instantiate(goalHolderPrefab, line.transform);
					goalHolder.transform.localPosition = new Vector3(-goalHolderLength * i, 0, 0);
					goalHolder.transform.rotation = line.rotation;
					goalHolder.Setup(goal.GoalColor.PersonType, goal.Count, j);
					goalHolder.OnComplete += OnGoalCompleted;

					lineQueues[j].Enqueue(goalHolder);
				}
			}
		}

		private void OnGoalCompleted(GoalHolder goalHolder)
		{
			goalHolder.OnComplete -= OnGoalCompleted;

			goalHolder.MoveTo(goalHolderMovePoint.position).OnComplete(() => { Destroy(goalHolder.gameObject); });

			var index = goalHolder.LineIndex;

			if (!lineQueues[index].TryDequeue(out var nextGoalHolder)) return;

			CurrentGoalHolders[index] = nextGoalHolder;
			nextGoalHolder.MoveTo(lines[index].position).OnComplete(() => OnNewGoal?.Invoke(nextGoalHolder));

			int i = 1;
			foreach (var holder in lineQueues[index])
			{
				holder.MoveTo(lines[index].position + i * goalHolderLength * Vector3.forward);
				i++;
			}
		}

		private void OnLevelStarted()
		{
			CurrentGoalHolders.Clear();

			for (int i = 0; i < LINE_COUNT; i++)
				CurrentGoalHolders.Add(lineQueues[i].Dequeue());
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

			if (goalHolder.Completed) return;

			StartCoroutine(GroupCompleteCoroutine(personGroup, goalHolder));
		}

		private IEnumerator GroupCompleteCoroutine(PersonGroup personGroup, GoalHolder goalHolder)
		{
			IsGoalSequence = true;
			personGroup.OpenCover();

			yield return StartCoroutine(goalHolder.SetPeople(personGroup));
			OnGoal?.Invoke();

			// yield return StartCoroutine(WaitForPackCompletion(personGroup, goalHolder));
		}

		private IEnumerator WaitForPackCompletion(PersonGroup personGroup, GoalHolder goalHolder)
		{
			yield return null;

			AudioManager.Instance.PlayAudio(AudioName.Goal);

			if (personGroup)
				StartCoroutine(personGroup.RemovePack());

			goalHolder.CloseCover().OnComplete(() =>
			{
				goalHolder.MoveTo(goalHolderMovePoint.position).OnComplete(() => { Destroy(goalHolder.gameObject); });

				IsGoalSequence = false;
			});
		}

		public GoalHolder GetCurrentGoalHolder(PersonType personType)
		{
			for (var i = 0; i < CurrentGoalHolders.Count; i++)
			{
				var currentGoalHolder = CurrentGoalHolders[i];
				if (!currentGoalHolder) continue;
				if (currentGoalHolder.PersonType == personType)
				{
					return currentGoalHolder;
				}
			}

			return null;
		}
	}
}