using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Fiber.AudioSystem;
using Fiber.Managers;
using Fiber.Utilities;
using GamePlay.People;
using GoalSystem;
using TriInspector;
using UnityEngine;
using UnityEngine.Events;
using Grid = GridSystem.Grid;

namespace Managers
{
	public class GoalManager : Singleton<GoalManager>
	{
		public bool IsGoalSequence { get; set; } = false;
		public GoalHolder CurrentGoalHolder { get; private set; }

		[SerializeField] private GoalHolder goalHolderPrefab;
		[Title("Points")]
		[SerializeField] private Transform goalHolderPoint;
		[SerializeField] private Transform goalHolderNextPoint;
		[SerializeField] private Transform goalHolderMovePoint;

		// private List<GoalHolder> holders = new List<GoalHolder>();
		private Dictionary<int, List<GoalHolder>> holders = new Dictionary<int, List<GoalHolder>>();

		private const float DELAY = .1F;

		public static event UnityAction OnGoal;

		private void OnEnable()
		{
			LevelManager.OnLevelLoad += OnLevelLoaded;
			StageManager.OnStageStarted += OnStageStarted;
			PersonGroup.OnComplete += OnBoltPackCompleted;
		}

		private void OnDisable()
		{
			LevelManager.OnLevelLoad -= OnLevelLoaded;
			StageManager.OnStageStarted -= OnStageStarted;
			PersonGroup.OnComplete -= OnBoltPackCompleted;
		}

		private void OnLevelLoaded()
		{
			for (var i = 0; i < LevelManager.Instance.CurrentLevelData.GoalStages.Length; i++)
			{
				var goalStage = LevelManager.Instance.CurrentLevelData.GoalStages[i];
				holders.Add(i, new List<GoalHolder>());
				foreach (var goal in goalStage.Goals)
				{
					var goalHolder = Instantiate(goalHolderPrefab, transform);
					goalHolder.Setup(goal.PersonType);
					goalHolder.gameObject.SetActive(false);

					holders[i].Add(goalHolder);
				}
			}
		}

		private void OnStageStarted(int stageIndex)
		{
			SpawnHolder(stageIndex);
		}

		private void SpawnHolder(int stageIndex)
		{
			if (!holders.TryGetValue(stageIndex, out var holderList))
			{
				CurrentGoalHolder = null;
				return;
			}

			if (holderList.Count <= 0)
			{
				CurrentGoalHolder = null;
				StageManager.Instance.StageComplete();
				return;
			}

			var holder = holderList[0];
			holderList.RemoveAt(0);
			CurrentGoalHolder = holder;

			holder.Spawn(goalHolderNextPoint).OnComplete(() =>
			{
				holder.MoveTo(goalHolderPoint.position).OnComplete(() =>
				{
					if (holderList.Count > 0)
					{
						var nextHolder = holderList[0];
						nextHolder.Spawn(goalHolderNextPoint);
					}
				});
			});
		}

		public void OnBoltPackCompleted(PersonGroup personGroup)
		{
			if (!CurrentGoalHolder) return;
			if (CurrentGoalHolder.Completed) return;
			if (personGroup.PersonGroupSlots[0].Person.PersonType != CurrentGoalHolder.PersonType)
			{
				personGroup.CloseCover();
				return;
			}

			CurrentGoalHolder.Completed = true;

			IsGoalSequence = true;
			personGroup.OpenCover();

			OnGoal?.Invoke();

			var bolts = personGroup.GetAllBolts().ToArray();
			for (var i = 0; i < bolts.Length; i++)
			{
				var bolt = bolts[i];
				CurrentGoalHolder.GoalSlots[i].SetPerson(bolt, false);

				var seq = DOTween.Sequence();
				// seq.Append(bolt.Unscrew());
				seq.Append(bolt.MoveToSlot(true).SetDelay(i * DELAY));
				var i1 = i;
				seq.AppendCallback(() =>
				{
					HapticManager.Instance.PlayHaptic(0.65f, 1f);
					AudioManager.Instance.PlayAudio(AudioName.Person).SetPitch(1 + i1 * 0.1f);
				});
				// seq.Append(bolt.Screw());
			}

			StartCoroutine(WaitForPackCompletion(personGroup));
		}

		private IEnumerator WaitForPackCompletion(PersonGroup boltPack)
		{
			var goalHolder = CurrentGoalHolder;

			yield return new WaitForSeconds(Person.MOVE_DURATION + Person.SCREW_DURATION * 2 + PersonGroup.MAX_PERSON_COUNT * DELAY);

			AudioManager.Instance.PlayAudio(AudioName.Goal);

			if (boltPack)
				StartCoroutine(boltPack.RemovePack());

			goalHolder.CloseCover().OnComplete(() =>
			{
				goalHolder.MoveTo(goalHolderMovePoint.position).OnComplete(() =>
				{
					Destroy(goalHolder.gameObject);

					Grid.Instance.CheckCompletedPacks();
				});
				SpawnHolder(StageManager.Instance.CurrentStageIndex);

				IsGoalSequence = false;
			});
		}
	}
}