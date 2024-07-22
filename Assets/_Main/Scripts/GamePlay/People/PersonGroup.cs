using System.Linq;
using System.Collections;
using System.Collections.Generic;
using DeckSystem;
using DG.Tweening;
using Fiber.Managers;
using Fiber.AudioSystem;
using Fiber.Utilities.Extensions;
using GridSystem;
using Interfaces;
using Lofelt.NiceVibrations;
using MoreMountains.Feedbacks;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

namespace GamePlay.People
{
	[SelectionBase]
	public class PersonGroup : MonoBehaviour, INode
	{
		public bool IsCompleted { get; private set; }
		public bool IsBusy { get; private set; } = false;
		public bool CanMove => currentDeckSlot && canMove;
		private bool canMove = false;

		[SerializeField] private Person personPrefab;
		[SerializeField] private PersonGroupSlot[] personGroupSlots;
		[Space]
		[SerializeField] private Transform model;
		[SerializeField] private Collider col;
		// [SerializeField] private Transform cover;

		[Space]
		[SerializeField] private MMF_Player feedback;

		public GridCell CurrentGridCell { get; set; }
		private DeckSlot currentDeckSlot;
		private GridCell currentNearestGridCell;
		public PersonGroupSlot[] PersonGroupSlots => personGroupSlots;

		private readonly List<GridCell> triggeredNodes = new List<GridCell>();

		private Rigidbody rb;

		public const int MAX_PERSON_COUNT = 6;

		public static event UnityAction<PersonGroup> OnPlace;
		public static event UnityAction<PersonGroup> OnComplete;

		private void Awake()
		{
			rb = GetComponent<Rigidbody>();

			for (int i = 0; i < personGroupSlots.Length; i++)
				personGroupSlots[i].Index = i;
		}

		private void OnDestroy()
		{
			transform.DOKill();
		}

		public void Init(List<LevelDataSO.PersonGroupSettings.PersonColor> personColors)
		{
			for (int i = 0; i < personColors.Count; i++)
			{
				if (personColors[i].PersonType == PersonType.None) continue;

				var person = Instantiate(personPrefab);
				person.Setup(personColors[i].PersonType);
				personGroupSlots[i].SetPerson(person);
			}
		}

		public void Setup(DeckSlot deckSlot, float moveDelay)
		{
			transform.SetParent(deckSlot.transform);
			currentDeckSlot = deckSlot;

			transform.DOLocalMoveX(0, .5f).SetEase(Ease.OutExpo).SetDelay(moveDelay).OnComplete(() => canMove = true);
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.attachedRigidbody && other.attachedRigidbody.TryGetComponent(out GridCell gridNode))
			{
				if (!gridNode.CurrentPersonGroup && gridNode.CurrentNode == null)
				{
					if (!triggeredNodes.Contains(gridNode))
					{
						triggeredNodes.Add(gridNode);
					}
				}
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.attachedRigidbody && other.attachedRigidbody.TryGetComponent(out GridCell gridNode))
			{
				if (triggeredNodes.Contains(gridNode))
				{
					triggeredNodes.Remove(gridNode);
					gridNode.HideHighlight();
				}
			}
		}

		public void OnPickUp()
		{
			for (var i = 0; i < personGroupSlots.Length; i++)
			{
				if (personGroupSlots[i].Person)
				{
					personGroupSlots[i].Person.OnGroupPickedUp();
				}
			}
		}

		public void OnRelease()
		{
			if (!CurrentGridCell && currentNearestGridCell && currentNearestGridCell.CurrentNode is null)
			{
				Place(currentNearestGridCell);
			}
			else
			{
				MoveBackToSlot();
			}
		}

		public void Place(GridCell placedCell)
		{
			CurrentGridCell = placedCell;
			CurrentGridCell.CurrentPersonGroup = this;
			CurrentGridCell.CurrentNode = this;
			currentDeckSlot = null;
			col.enabled = false;

			transform.SetParent(placedCell.transform);
			transform.DOLocalMove(Vector3.zero, .25f).SetEase(Ease.OutBack).OnComplete(() =>
			{
				HapticManager.Instance.PlayHaptic(0.5f, 0.5f);

				for (var i = 0; i < personGroupSlots.Length; i++)
				{
					if (personGroupSlots[i].Person)
						personGroupSlots[i].Person.OnGroupPlaced();
				}

				OnPlace?.Invoke(this);
			});

			ResetRotation();

			placedCell.HideHighlight();
		}

		public void MoveBackToSlot()
		{
			if (currentNearestGridCell)
				currentNearestGridCell.HideHighlight();
			currentNearestGridCell = null;

			HapticManager.Instance.PlayHaptic(HapticPatterns.PresetType.Warning);

			canMove = false;
			ResetRotation();
			transform.DOLocalMove(Vector3.zero, .2f).SetEase(Ease.OutExpo).OnComplete(() => canMove = true);

			for (var i = 0; i < personGroupSlots.Length; i++)
			{
				if (personGroupSlots[i].Person)
					personGroupSlots[i].Person.OnGroupDroppedDown();
			}
		}

		public void Move(Vector3 position)
		{
			rb.MovePosition(Vector3.Lerp(rb.position, position, Time.deltaTime * 10));

			var nearestCell = GetNearestNode();
			if (currentNearestGridCell)
			{
				if (!currentNearestGridCell.Equals(nearestCell))
				{
					currentNearestGridCell.HideHighlight();
				}
			}

			currentNearestGridCell = nearestCell;
			if (currentNearestGridCell && !currentNearestGridCell.IsShowingHighlight)
			{
				currentNearestGridCell.ShowHighlight();
				HapticManager.Instance.PlayHaptic(0.3f, 0);
			}

			for (var i = 0; i < personGroupSlots.Length; i++)
			{
				if (personGroupSlots[i].Person)
				{
					personGroupSlots[i].Person.OnGroupMove();
				}
			}
		}

		private void ResetRotation()
		{
			model.DORotateQuaternion(Quaternion.identity, .1f);
		}

		// [Button("Rearrange")]
		public void Rearrange(bool changePosition = true)
		{
			// Rearrange empty slots
			var emptySlots = GetEmptySlots().ToList();
			if (!emptySlots.Count.Equals(MAX_PERSON_COUNT))
			{
				int emptySlotCount = 1;
				foreach (var emptySlot in emptySlots)
				{
					for (int i = MAX_PERSON_COUNT - emptySlotCount; i >= 0; i--)
					{
						if (personGroupSlots[i].Person && emptySlot.Index < i)
						{
							personGroupSlots[i].Person.ChangeSlot(emptySlot, true, changePosition);
							emptySlotCount++;
							break;
						}
					}
				}
			}

			int typeCount = GetPersonTypes().Count;
			int index = 0;
			int p = -1;
			for (int i = 0; i < typeCount; i++)
			{
				var firstSlot = personGroupSlots[index];
				if (!firstSlot.Person) break;

				// start searching from the next index
				for (int j = index + 1; j < MAX_PERSON_COUNT; j++)
				{
					// check if the selected index and traversed index are same type
					if (personGroupSlots[j].Person?.PersonType == firstSlot.Person.PersonType)
					{
						// swap if not the same slot
						if (p != -1 && !personGroupSlots[j].Equals(personGroupSlots[p]))
						{
							var tempBolt = personGroupSlots[p].Person;
							personGroupSlots[j].Person?.ChangeSlot(personGroupSlots[p], true, changePosition);
							tempBolt?.ChangeSlot(personGroupSlots[j], false, changePosition);

							p++;
						}
					}
					else if (p == -1)
					{
						p = j;
					}
				}

				// select the current pointer because it's different
				index = p;
				// start searching from next index
				p++;
			}
		}

		public IEnumerator MovePeople()
		{
			yield return new WaitUntil(() => !IsBusy);
			if (!isActiveAndEnabled) yield break;
			IsBusy = true;

			for (var i = 0; i < personGroupSlots.Length; i++)
			{
				var slot = personGroupSlots[i];
				if (!slot.Person) continue;

				slot.Person.MoveToSlot(true);
			}

			var isCompleted = CheckIfSorted();

			var filledSlots = personGroupSlots.Where(x => x.Person);
			yield return new WaitUntil(() => !filledSlots.Any(x => x.Person.IsMoving));
			yield return null;

			IsBusy = false;
			if (isCompleted)
			{
				Complete();
			}
		}

		public bool CheckIfSorted()
		{
			int count = 0;
			int samePersonTypeCount = 1;

			for (int i = 0; i < personGroupSlots.Length; i++)
			{
				if (!personGroupSlots[i].Person) continue;

				count++;

				if (i > 0 && personGroupSlots[i - 1].Person?.PersonType == personGroupSlots[i].Person.PersonType)
					samePersonTypeCount++;
			}

			// Remove pack from grid
			if (count.Equals(0))
			{
				StartCoroutine(RemovePack());
			}

			// Pack it up
			return samePersonTypeCount.Equals(MAX_PERSON_COUNT);
		}

		private void Complete()
		{
			IsCompleted = true;
			OnComplete?.Invoke(this);
		}

		public void CloseCover()
		{
			//TODO: more feedbacks
			feedback.PlayFeedbacks();
		}

		public IEnumerator RemovePack()
		{
			if (!gameObject) yield break;

			transform.DOScale(0, .5f).SetEase(Ease.InBack).OnComplete(() =>
			{
				CurrentGridCell.CurrentPersonGroup = null;
				CurrentGridCell.CurrentNode = null;
				if (gameObject)
					Destroy(gameObject);
			});
		}

		private GridCell GetNearestNode()
		{
			if (triggeredNodes.Count.Equals(0)) return null;

			GridCell nearestCell = null;
			var shortestDistance = float.MaxValue;
			for (int i = 0; i < triggeredNodes.Count; i++)
			{
				if ((transform.position - triggeredNodes[i].transform.position).sqrMagnitude < shortestDistance)
				{
					nearestCell = triggeredNodes[i];
					shortestDistance = (transform.position - nearestCell.transform.position).sqrMagnitude;
				}
			}

			return nearestCell;
		}

		public IEnumerable<Person> GetPeopleByType(PersonType personType)
		{
			for (int i = 0; i < personGroupSlots.Length; i++)
				if (personGroupSlots[i].Person && personGroupSlots[i].Person.PersonType == personType)
					yield return personGroupSlots[i].Person;
		}

		public int GetPersonCountByType(PersonType boltType)
		{
			int count = 0;
			for (int i = 0; i < personGroupSlots.Length; i++)
			{
				if (personGroupSlots[i].Person && personGroupSlots[i].Person.PersonType == boltType)
					count++;
			}

			return count;
		}

		public int GetPeopleCount()
		{
			int count = 0;
			for (int i = 0; i < personGroupSlots.Length; i++)
			{
				if (personGroupSlots[i].Person)
					count++;
			}

			return count;
		}

		public List<PersonType> GetPersonTypes()
		{
			var boltTypes = new List<PersonType>();
			for (int i = 0; i < personGroupSlots.Length; i++)
				if (personGroupSlots[i].Person)
					boltTypes.AddIfNotContains(personGroupSlots[i].Person.PersonType);

			return boltTypes;
		}

		public List<PersonType> GetPersonTypesOrdered(bool descending = false)
		{
			var boltTypesCount = new Dictionary<PersonType, int>();
			for (int i = 0; i < personGroupSlots.Length; i++)
			{
				if (!personGroupSlots[i].Person) continue;
				if (!boltTypesCount.TryAdd(personGroupSlots[i].Person.PersonType, 1))
					boltTypesCount[personGroupSlots[i].Person.PersonType]++;
			}

			var max = descending ? boltTypesCount.OrderByDescending(x => x.Value).Select(y => y.Key).ToList() : boltTypesCount.OrderBy(x => x.Value).Select(y => y.Key).ToList();
			return max;
		}

		public IEnumerable<PersonGroupSlot> GetEmptySlots()
		{
			for (int i = 0; i < personGroupSlots.Length; i++)
				if (!personGroupSlots[i].Person)
					yield return personGroupSlots[i];
		}

		public IEnumerable<Person> GetAllPeople()
		{
			for (int i = 0; i < personGroupSlots.Length; i++)
				if (personGroupSlots[i].Person)
					yield return personGroupSlots[i].Person;
		}

		public bool ContainsPersonType(PersonType boltType)
		{
			for (int i = 0; i < personGroupSlots.Length; i++)
				if (personGroupSlots[i].Person?.PersonType == boltType)
					return true;

			return false;
		}
	}
}