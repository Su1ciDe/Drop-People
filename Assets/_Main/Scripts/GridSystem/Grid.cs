using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Fiber.Managers;
using Fiber.Utilities;
using GamePlay.People;
using GamePlay.Obstacles;
using Managers;
using UnityEngine;

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
		[SerializeField] private Obstacle obstaclePrefab;

		private GridCell[,] gridCells;
		public GridCell[,] GridCells => gridCells;

		private void Awake()
		{
			Setup();
		}

		private void OnEnable()
		{
			PersonGroup.OnPlace += OnBoltPlaced;
			LevelManager.OnLevelLoad += OnLevelLoaded;
			StageManager.OnStageStarted += OnStageStarted;
			GoalManager.OnGoal += OnGoal;
		}

		private void OnDisable()
		{
			PersonGroup.OnPlace -= OnBoltPlaced;
			LevelManager.OnLevelLoad -= OnLevelLoaded;
			StageManager.OnStageStarted -= OnStageStarted;
			GoalManager.OnGoal -= OnGoal;
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
					if (LevelManager.Instance.CurrentLevelData.Obstacles.GetCell(x, y))
					{
						var obstacle = Instantiate(obstaclePrefab, transform);
						obstacle.Place(gridCells[x, y]);
					}
				}
			}
		}

		private void OnBoltPlaced(PersonGroup placedPersonGroup)
		{
			var index = placedPersonGroup.CurrentGridCell.Coordinates;
			var connectedPersonGroups = new List<PersonGroup>();
			// Check neighbours

			// Left
			if (!index.x.Equals(0))
				CheckHasSameType(gridCells[index.x - 1, index.y].CurrentPersonGroup, ref connectedPersonGroups);

			// Right
			if (!index.x.Equals(size.x - 1))
				CheckHasSameType(gridCells[index.x + 1, index.y].CurrentPersonGroup, ref connectedPersonGroups);

			// Up
			if (!index.y.Equals(0))
				CheckHasSameType(gridCells[index.x, index.y - 1].CurrentPersonGroup, ref connectedPersonGroups);

			// Down
			if (!index.y.Equals(size.y - 1))
				CheckHasSameType(gridCells[index.x, index.y + 1].CurrentPersonGroup, ref connectedPersonGroups);

			// Add original pack
			if (connectedPersonGroups.Count > 0 && !connectedPersonGroups.Contains(placedPersonGroup))
				connectedPersonGroups.Add(placedPersonGroup);

			if (connectedPersonGroups.Count <= 1) return;

			// Select the most occuring type
			connectedPersonGroups = connectedPersonGroups.OrderBy(y => y.GetPersonTypes().Count).ToList();
			foreach (var personGroup in connectedPersonGroups)
			{
				var selectedType = PersonType.None;
				var types = personGroup.GetPersonTypesOrdered(true);
				for (int i = 0; i < types.Count; i++)
				{
					selectedType = types[i];
					bool selected = false;
					for (int j = 0; j < connectedPersonGroups.Count; j++)
					{
						if (connectedPersonGroups[j].Equals(personGroup)) continue;
						if (!connectedPersonGroups[j].ContainsBoltType(selectedType)) continue;

						selected = true;
						break;
					}

					if (selected)
						break;
				}

				// Sort
				for (int i = 0; i < personGroup.PersonGroupSlots.Length; i++)
				{
					if (personGroup.PersonGroupSlots[i].Person?.PersonType == selectedType) continue;

					int pointer = 0;
					var selectedPerson = personGroup.PersonGroupSlots[i].Person;

					for (var j = 0; j < connectedPersonGroups.Count; j++)
					{
						if (pointer >= PersonGroup.MAX_PERSON_COUNT)
						{
							pointer = 0;
							continue;
						}

						var otherPack = connectedPersonGroups[j];
						if (otherPack.Equals(personGroup)) continue;

						var otherPacksBoltCount = otherPack.GetBoltCountByType(selectedType);
						if (!otherPack.GetBoltsCount().Equals(PersonGroup.MAX_PERSON_COUNT) && otherPacksBoltCount > personGroup.GetBoltCountByType(selectedType))
						{
							pointer = 0;
							continue;
						}

						if (otherPacksBoltCount.Equals(PersonGroup.MAX_PERSON_COUNT))
						{
							pointer = 0;
							continue;
						}

						var otherPacksTypes = otherPack.GetPersonTypes();

						if (selectedPerson && otherPacksTypes.Contains(selectedPerson.PersonType) && otherPack.PersonGroupSlots[pointer].Person?.PersonType == selectedType)
						{
							otherPack.PersonGroupSlots[pointer].Person.ChangeSlot(personGroup.PersonGroupSlots[i], true, false);
							selectedPerson.ChangeSlot(otherPack.PersonGroupSlots[pointer], true, false);
							break;
						}
						else if (!selectedPerson && otherPack.PersonGroupSlots[pointer].Person?.PersonType == selectedType)
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

			// Rearrange
			foreach (var boltPackPair in connectedPersonGroups)
			{
				boltPackPair.Rearrange(false);
			}

			// Moving Sequence
			foreach (var boltPackPair in connectedPersonGroups)
			{
				StartCoroutine(boltPackPair.MoveBolts());
			}

			if (moveSequenceCoroutine is not null)
			{
				StopCoroutine(moveSequenceCoroutine);
				moveSequenceCoroutine = null;
			}
			moveSequenceCoroutine = StartCoroutine(MoveSequence());
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

		private IEnumerator MoveSequence()
		{
			yield return new WaitForSeconds(Person.MOVE_DURATION + Person.SCREW_DURATION * 2);
			yield return null;
			yield return new WaitUntil(() => !GoalManager.Instance.IsGoalSequence);
			yield return new WaitForSeconds(2.5f);

			CheckIfFailed();
		}

		private void CheckIfFailed()
		{
			if (GoalManager.Instance.IsGoalSequence) return;

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

		public void CheckCompletedPacks()
		{
			if (!GoalManager.Instance.CurrentGoalHolder) return;

			PersonGroup completedPersonGroup = null;
			for (int x = 0; x < size.x; x++)
			{
				for (int y = 0; y < size.y; y++)
				{
					var grid = gridCells[x, y];
					if (grid.CurrentPersonGroup && grid.CurrentPersonGroup.IsCompleted && grid.CurrentPersonGroup.PersonGroupSlots[0].Person.PersonType == GoalManager.Instance.CurrentGoalHolder.PersonType)
					{
						completedPersonGroup = grid.CurrentPersonGroup;
						break;
					}
				}

				if (completedPersonGroup)
					break;
			}

			if (completedPersonGroup)
				GoalManager.Instance.OnBoltPackCompleted(completedPersonGroup);
		}

		private void CheckHasSameType(PersonGroup neighbourGroup, ref List<PersonGroup> connectedPersonGroups)
		{
			if (neighbourGroup && !neighbourGroup.IsCompleted && !connectedPersonGroups.Contains(neighbourGroup))
			{
				connectedPersonGroups.Add(neighbourGroup);
			}
		}

		private void OnStageStarted(int stageNo)
		{
			if (stageNo.Equals(0)) return;

			StartCoroutine(WaitStage());
			return;

			IEnumerator WaitStage()
			{
				yield return new WaitForSeconds(1);
				CheckCompletedPacks();
			}
		}
	}
}