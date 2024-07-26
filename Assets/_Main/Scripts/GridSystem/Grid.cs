using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Fiber.Managers;
using Fiber.Utilities;
using GamePlay.People;
using GamePlay.Obstacles;
using GoalSystem;
using Managers;
using ScriptableObjects;
using UnityEngine;
using Utilities;

namespace GridSystem
{
	public class Grid : Singleton<Grid>
	{
		[SerializeField] private Vector2Int size = new Vector2Int(4, 6);
		[SerializeField] private Vector2 nodeSize;
		[SerializeField] private float xSpacing = .1f;
		[SerializeField] private float ySpacing = .1f;
		[SerializeField] private GridCell cellPrefab;
		[Space]
		[SerializeField] private ObstaclesSO obstaclesSO;

		private GridCell[,] gridCells;
		public GridCell[,] GridCells => gridCells;

		private void Awake()
		{
			Setup();
		}

		private void OnEnable()
		{
			PersonGroup.OnPlace += OnPersonGroupPlaced;
			LevelManager.OnLevelLoad += OnLevelLoaded;
			GoalManager.OnGoal += OnGoal;
			GoalManager.OnNewGoal += CheckCompletedPacks;
		}

		private void OnDisable()
		{
			PersonGroup.OnPlace -= OnPersonGroupPlaced;
			LevelManager.OnLevelLoad -= OnLevelLoaded;
			GoalManager.OnGoal -= OnGoal;
			GoalManager.OnNewGoal -= CheckCompletedPacks;
		}

		private void Setup()
		{
			gridCells = new GridCell[size.x, size.y];

			var xOffset = (nodeSize.x * size.x + xSpacing * (size.x - 1)) / 2f - nodeSize.x / 2f;
			var yOffset = (nodeSize.y * size.y + ySpacing * (size.y - 1)) / 2f - nodeSize.y / 2f;
			for (int y = 0; y < size.y; y++)
			{
				for (int x = 0; x < size.x; x++)
				{
					var node = Instantiate(cellPrefab, transform);
					node.Setup(x, y, nodeSize);
					node.gameObject.name = x + " - " + y;
					node.transform.localPosition = new Vector3(x * (nodeSize.x + xSpacing) - xOffset, 0, -y * (nodeSize.y + ySpacing) + yOffset);
					gridCells[x, y] = node;
				}
			}
		}

		private void OnLevelLoaded()
		{
			for (int y = 0; y < size.y; y++)
			{
				for (int x = 0; x < size.x; x++)
				{
					var obstacleType = LevelManager.Instance.CurrentLevelData.Obstacles.GetCell(x, y);
					if (obstacleType != LevelEditorEnum.Grid)
					{
						var obstacle = Instantiate(obstaclesSO.Obstacles[obstacleType], transform);
						obstacle.Place(gridCells[x, y]);

						if (obstacleType == LevelEditorEnum.Empty)
							gridCells[x, y].Model.SetActive(false);
					}
				}
			}
		}

		private void OnPersonGroupPlaced(PersonGroup placedPersonGroup)
		{
			var coordinates = placedPersonGroup.CurrentGridCell.Coordinates;
			var connectedPersonGroups = new List<PersonGroup>();
			// Check neighbors

			// Left
			if (!coordinates.x.Equals(0))
				CheckHasSameType(placedPersonGroup, gridCells[coordinates.x - 1, coordinates.y].CurrentPersonGroup, ref connectedPersonGroups);

			// Right
			if (!coordinates.x.Equals(size.x - 1))
				CheckHasSameType(placedPersonGroup, gridCells[coordinates.x + 1, coordinates.y].CurrentPersonGroup, ref connectedPersonGroups);

			// Up
			if (!coordinates.y.Equals(0))
				CheckHasSameType(placedPersonGroup, gridCells[coordinates.x, coordinates.y - 1].CurrentPersonGroup, ref connectedPersonGroups);

			// Down
			if (!coordinates.y.Equals(size.y - 1))
				CheckHasSameType(placedPersonGroup, gridCells[coordinates.x, coordinates.y + 1].CurrentPersonGroup, ref connectedPersonGroups);

			// Add the original pack
			if (connectedPersonGroups.Count > 0 && !connectedPersonGroups.Contains(placedPersonGroup))
				connectedPersonGroups.Add(placedPersonGroup);

			if (connectedPersonGroups.Count <= 1)
			{
				if (moveSequenceCoroutine is not null)
				{
					StopCoroutine(moveSequenceCoroutine);
					moveSequenceCoroutine = null;
				}

				moveSequenceCoroutine = StartCoroutine(MoveSequence(connectedPersonGroups));
				return;
			}

			Sort(ref connectedPersonGroups);
			Sort(ref connectedPersonGroups);

			// Rearrange
			foreach (var personGroup in connectedPersonGroups)
			{
				personGroup.Rearrange(false);
			}

			// Moving Sequence
			foreach (var personGroup in connectedPersonGroups)
			{
				StartCoroutine(personGroup.MovePeople());
			}

			if (moveSequenceCoroutine is not null)
			{
				StopCoroutine(moveSequenceCoroutine);
				moveSequenceCoroutine = null;
			}

			moveSequenceCoroutine = StartCoroutine(MoveSequence(connectedPersonGroups));
		}

		private void Sort(ref List<PersonGroup> connectedPersonGroups)
		{
			// Select groups by the most occuring type
			connectedPersonGroups = connectedPersonGroups.OrderBy(y => y.GetPersonTypes().Count).ToList();
			foreach (var personGroup in connectedPersonGroups)
			{
				var selectedType = PersonType.None;
				// Select types by the most count
				var types = personGroup.GetPersonTypesOrdered(true);
				for (int i = 0; i < types.Count; i++)
				{
					selectedType = types[i];
					bool selected = false;
					for (int j = 0; j < connectedPersonGroups.Count; j++)
					{
						if (connectedPersonGroups[j].Equals(personGroup)) continue;
						if (!connectedPersonGroups[j].ContainsPersonType(selectedType)) continue;

						selected = true;
						break;
					}

					if (selected) break;
				}

				// Sort
				for (int i = 0; i < personGroup.PersonGroupSlots.Length; i++)
				{
					var selectedPerson = personGroup.PersonGroupSlots[i].Person;
					// if (selectedPerson?.PersonType == selectedType) continue;

					int pointer = 0;
					for (var j = 0; j < connectedPersonGroups.Count; j++)
					{
						var otherGroup = connectedPersonGroups[j];
						if (otherGroup.Equals(personGroup))
						{
							pointer = 0;
							continue;
						}

						if (pointer >= PersonGroup.MAX_PERSON_COUNT)
						{
							pointer = 0;
							continue;
						}

						var otherGroupsPersonCountByType = otherGroup.GetPersonCountByType(selectedType);
						if (otherGroupsPersonCountByType.Equals(PersonGroup.MAX_PERSON_COUNT))
						{
							pointer = 0;
							continue;
						}

						Debug.Log(selectedPerson?.PersonType);
						var groupsPersonCountByType = personGroup.GetPersonCountByType(selectedType);
						if (!otherGroup.GetPeopleCount().Equals(PersonGroup.MAX_PERSON_COUNT) && otherGroupsPersonCountByType > groupsPersonCountByType)
						{
							pointer = 0;
							continue;
						}

						var otherPacksTypes = otherGroup.GetPersonTypes();
						if (selectedPerson && selectedPerson.PersonType != selectedType && otherGroup.ContainsPersonType(selectedPerson.PersonType) && !otherGroup.PersonGroupSlots[pointer].Person)
						{
							selectedPerson.ChangeSlot(otherGroup.PersonGroupSlots[pointer], true, false);
							break;
						}
						else if (selectedPerson && selectedPerson.PersonType != selectedType && otherGroup.PersonGroupSlots[pointer].Person?.PersonType == selectedType)
						{
							otherGroup.PersonGroupSlots[pointer].Person.ChangeSlot(personGroup.PersonGroupSlots[i], true, false);
							selectedPerson.ChangeSlot(otherGroup.PersonGroupSlots[pointer], true, false);
							break;
						}
						else if (selectedPerson && otherPacksTypes.Contains(selectedPerson.PersonType) && otherGroup.PersonGroupSlots[pointer].Person?.PersonType == selectedType)
						{
							otherGroup.PersonGroupSlots[pointer].Person.ChangeSlot(personGroup.PersonGroupSlots[i], true, false);
							selectedPerson.ChangeSlot(otherGroup.PersonGroupSlots[pointer], true, false);
							break;
						}
						else if (!selectedPerson && otherGroup.PersonGroupSlots[pointer].Person?.PersonType == selectedType)
						{
							connectedPersonGroups[j].PersonGroupSlots[pointer].Person.ChangeSlot(personGroup.PersonGroupSlots[i], true, false);
							break;
						}
						else
						{
							pointer++;
							j--;
						}
					}
				}
			}
		}

		private void OnGoal()
		{
			if (moveSequenceCoroutine is not null)
			{
				StopCoroutine(moveSequenceCoroutine);
				moveSequenceCoroutine = null;
			}
		}

		private Coroutine moveSequenceCoroutine = null;

		private IEnumerator MoveSequence(List<PersonGroup> connectedPersonGroups)
		{
			var people = connectedPersonGroups.SelectMany(x => x.PersonGroupSlots.Where(y => y.Person)).Select(z => z.Person);
			yield return null;
			yield return new WaitUntil(() => !people.Any(x => x.IsMoving));
			yield return new WaitForSeconds(0.25f);
			yield return new WaitUntil(() => !GoalManager.Instance.IsGoalSequence);
			yield return null;

			yield return new WaitUntil(() => !GoalManager.Instance.IsGoalSequence);
			yield return null;

			int filledNodeCount = 0;
			for (int x = 0; x < size.x; x++)
			{
				for (int y = 0; y < size.y; y++)
				{
					if (gridCells[x, y].CurrentPersonGroup || gridCells[x, y].CurrentNode != null)
						filledNodeCount++;
				}
			}

			if (filledNodeCount.Equals(gridCells.Length))
			{
				LevelManager.Instance.Lose();
			}
		}

		public void CheckCompletedPacks(GoalHolder goalHolder)
		{
			for (int x = 0; x < size.x; x++)
			{
				for (int y = 0; y < size.y; y++)
				{
					var grid = gridCells[x, y];
					if (grid.CurrentPersonGroup && grid.CurrentPersonGroup.IsCompleted && grid.CurrentPersonGroup.PersonGroupSlots[0].Person.PersonType == goalHolder.PersonType &&
					    !goalHolder.IsCompleted)
					{
						GoalManager.Instance.OnPersonGroupCompleted(grid.CurrentPersonGroup);
					}
				}
			}
		}

		private void CheckHasSameType(PersonGroup placedPersonGroup, PersonGroup neighbourGroup, ref List<PersonGroup> connectedPersonGroups)
		{
			if (neighbourGroup && !neighbourGroup.IsCompleted && !connectedPersonGroups.Contains(neighbourGroup))
			{
				if (HasSameType(placedPersonGroup, neighbourGroup))
					connectedPersonGroups.Add(neighbourGroup);
			}
		}

		private bool HasSameType(PersonGroup placedPersonGroup, PersonGroup neighbourGroup)
		{
			var placedTypes = placedPersonGroup.GetPersonTypes();

			for (var i = 0; i < placedTypes.Count; i++)
			{
				if (neighbourGroup.ContainsPersonType(placedTypes[i]))
					return true;
			}

			return false;
		}

		public void CheckObstacles(PersonGroup personGroup)
		{
			var coordinates = personGroup.CurrentGridCell.Coordinates;

			// Check neighbor obstacles
			// Left
			if (!coordinates.x.Equals(0))
				if (gridCells[coordinates.x - 1, coordinates.y].CurrentNode is BaseObstacle obstacle)
					obstacle.OnGroupCompleteNear(personGroup);

			// Right
			if (!coordinates.x.Equals(size.x - 1))
				if (gridCells[coordinates.x + 1, coordinates.y].CurrentNode is BaseObstacle obstacle)
					obstacle.OnGroupCompleteNear(personGroup);

			// Up
			if (!coordinates.y.Equals(0))
				if (gridCells[coordinates.x, coordinates.y - 1].CurrentNode is BaseObstacle obstacle)
					obstacle.OnGroupCompleteNear(personGroup);

			// Down
			if (!coordinates.y.Equals(size.y - 1))
				if (gridCells[coordinates.x, coordinates.y + 1].CurrentNode is BaseObstacle obstacle)
					obstacle.OnGroupCompleteNear(personGroup);
		}

		public GridCell GetFirstEmptyCell()
		{
			for (int x = 0; x < size.x; x++)
			{
				for (int y = 0; y < size.y; y++)
				{
					if (gridCells[x, y].CurrentNode is null)
					{
						return gridCells[x, y];
					}
				}
			}

			return null;
		}
	}
}