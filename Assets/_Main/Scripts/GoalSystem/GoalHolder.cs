using System.Linq;
using System.Collections;
using DG.Tweening;
using Fiber.Managers;
using Fiber.AudioSystem;
using GamePlay.People;
using Lofelt.NiceVibrations;
using Managers;
using MoreMountains.Feedbacks;
using PathCreation;
using TMPro;
using TriInspector;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace GoalSystem
{
	[SelectionBase]
	public class GoalHolder : MonoBehaviour
	{
		public bool IsCompleted { get; private set; } = false;
		public PersonType PersonType { get; private set; }
		public int NeededAmount { get; private set; }
		public int LineIndex { get; private set; }

		[SerializeField] private MeshRenderer holderMeshRenderer;
		[Space]
		[SerializeField] private GoalSlot[] goalSlots;
		public GoalSlot[] GoalSlots => goalSlots;

		[SerializeField] private Transform entrancePoint;
		[Space]
		[SerializeField] private MMF_Player feedbacks;

		[Title("UI")]
		[SerializeField] private Canvas progressBarUI;
		[SerializeField] private SlicedFilledImage imgProgressBar;
		[SerializeField] private TMP_Text txtGoalCount;

		private int currentAmount;
		private int currentPersonAmount;

		private NavMeshObstacle[] navMeshObstacles;

		private const float MOVE_DURATION = .35F;
		private const int MATERIAL_INDEX = 0;

		public event UnityAction<GoalHolder> OnComplete;

		private void Awake()
		{
			navMeshObstacles = GetComponentsInChildren<NavMeshObstacle>();
		}

		private void OnDisable()
		{
			transform.DOKill();
		}

		public IEnumerator SetPeople(PersonGroup personGroup)
		{
			var people = personGroup.GetAllPeople().ToArray();
			var peopleCount = people.Length;

			for (var i = 0; i < peopleCount; i++)
			{
				var slot = GetFirstGoalSlot();
				if (!slot) continue;

				slot.SetPerson(people[i], false);
				currentAmount++;
			}

			IsCompleted = CheckIfCompleted();

			yield return null;
			for (var i = 0; i < peopleCount; i++)
			{
				yield return new WaitForSeconds(GoalManager.DELAY);
				people[i].MoveToSlot(true);
				StartCoroutine(FeedbackCoroutine(people[i], i));
			}

			if (personGroup)
				StartCoroutine(personGroup.RemovePack());

			yield return null;
			yield return new WaitUntil(() => !people.Any(x => x.IsMoving));
			yield return null;

			for (var i = 0; i < peopleCount; i++)
			{
				people[i].Agent.enabled = false;
			}

			if (IsCompleted)
			{
				progressBarUI.gameObject.SetActive(false);
				feedbacks.PlayFeedbacks();
				OnComplete?.Invoke(this);
			}

			imgProgressBar.FillAmount = (float)currentAmount / NeededAmount;
			txtGoalCount.SetText(currentAmount.ToString() + "/" + NeededAmount.ToString());
		}

		private IEnumerator FeedbackCoroutine(Person person, int index)
		{
			yield return new WaitUntil(() => !person.IsMoving);

			holderMeshRenderer.transform.DOComplete();
			holderMeshRenderer.transform.DOScale(1.1f * Vector3.one, .1f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutExpo);

			// AudioManager.Instance.PlayAudio(AudioName.Goal).SetPitch(1 + .1f * index);
			AudioManager.Instance.PlayAudio(AudioName.Pop1).SetPitch(1 + .1f * index);
			HapticManager.Instance.PlayHaptic(HapticPatterns.PresetType.RigidImpact);

			currentPersonAmount++;
			imgProgressBar.FillAmount = (float)currentPersonAmount / NeededAmount;
			txtGoalCount.SetText(currentPersonAmount.ToString() + "/" + NeededAmount.ToString());
		}

		private bool CheckIfCompleted()
		{
			return currentAmount >= NeededAmount;
		}

		public void Setup(PersonType personType, int neededAmount, int lineIndex)
		{
			PersonType = personType;
			NeededAmount = neededAmount;
			LineIndex = lineIndex;
			currentAmount = 0;

			var mat = GameManager.Instance.PersonMaterialsSO.GoalHolderMaterials[personType];
			var mats = holderMeshRenderer.materials;
			mats[MATERIAL_INDEX] = mat;
			holderMeshRenderer.materials = mats;

			txtGoalCount.SetText(currentAmount.ToString() + "/" + NeededAmount.ToString());
		}

		public Tween Spawn(Transform point)
		{
			var duration = gameObject.activeSelf ? 0 : .25f;

			gameObject.SetActive(true);
			transform.position = point.position;
			transform.rotation = point.rotation;

			return transform.DOScale(0, duration).From().SetEase(Ease.OutBack);
		}

		public void OnCurrentGoal()
		{
			progressBarUI.gameObject.SetActive(true);
		}

		public Tween MoveTo(Vector3 position)
		{
			return transform.DOMove(position, MOVE_DURATION).SetEase(Ease.InOutQuart);
		}

		private float distanceTravelled;
		private const float SPEED = 35;

		public IEnumerator MoveToEndCoroutine(VertexPath path)
		{
			foreach (var navMeshObstacle in navMeshObstacles)
				navMeshObstacle.enabled = false;

			while (path.GetClosestTimeOnPath(transform.position) < 1)
			{
				distanceTravelled += SPEED * Time.deltaTime;
				transform.position = path.GetPointAtDistance(distanceTravelled, EndOfPathInstruction.Stop);
				transform.rotation = path.GetRotationAtDistance(distanceTravelled, EndOfPathInstruction.Stop);
				yield return null;
			}

			yield return null;

			Destroy(gameObject);
		}

		public GoalSlot GetFirstGoalSlot()
		{
			for (var i = 0; i < goalSlots.Length; i++)
			{
				if (!goalSlots[i].Person)
					return goalSlots[i];
			}

			return null;
		}
	}
}