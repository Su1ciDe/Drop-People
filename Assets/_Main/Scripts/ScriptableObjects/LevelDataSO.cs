using System;
using System.Linq;
using System.Collections.Generic;
using Fiber.Utilities.Extensions;
using GamePlay.People;
using TriInspector;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace ScriptableObjects
{
	[CreateAssetMenu(fileName = "Level_001", menuName = "DropPeople/Level Data", order = 0)]
	[DeclareFoldoutGroup("Randomizer")]
	[DeclareHorizontalGroup("Randomizer/count")]
	public class LevelDataSO : ScriptableObject
	{
		[Title("Bolt Packs")]
		[ListDrawerSettings(ShowElementLabels = true), OnValueChanged(nameof(CalculateCount))]
		public PersonGroupSettings[] BoltPacks;

		[TableList(Draggable = false, AlwaysExpanded = true, HideAddButton = true, HideRemoveButton = true, ShowElementLabels = false)]
		[SerializeField] private List<PersonCount> personCounts;

		[Serializable]
		private class PersonCount
		{
			[GUIColor("$GetColor")]
			[ReadOnly] public PersonType PersonType;
			[ReadOnly] public int Count;

			public PersonCount(PersonType personType, int count)
			{
				PersonType = personType;
				Count = count;
			}

			private Color GetColor
			{
				get
				{
					var color = PersonType switch
					{
						PersonType.Blue => Color.blue,
						PersonType.Green => Color.green,
						PersonType.Orange => new Color(1f, 0.5f, 0),
						PersonType.Pink => Color.magenta,
						PersonType.Purple => new Color(.7f, .25f, 1f),
						PersonType.Red => Color.red,
						PersonType.Yellow => Color.yellow,
						PersonType.None => Color.white,
						_ => throw new ArgumentOutOfRangeException()
					};

					return color;
				}
			}
		}

		[Group("Randomizer")] [SerializeField] private int count;
		[Group("Randomizer/count")] [SerializeField] private int minPersonCount;
		[Group("Randomizer/count")] [SerializeField] private int maxPersonCount;
		[Group("Randomizer")] [SerializeField] private List<PersonGroupSettings.PersonColor> personTypes;

		[Group("Randomizer"), Button(ButtonSizes.Medium)]
		private void Randomize()
		{
			BoltPacks = new PersonGroupSettings[count];
			for (int i = 0; i < count; i++)
			{
				BoltPacks[i] = new PersonGroupSettings();
				var r = Random.Range(minPersonCount, maxPersonCount + 1);
				BoltPacks[i].PersonTypes = new List<PersonGroupSettings.PersonColor>(r);
				for (int j = 0; j < r; j++)
				{
					BoltPacks[i].PersonTypes.Add(personTypes.RandomItem());
				}
			}
		}

		[Title("Goals")]
		public GoalSettings[] GoalStages;

		[Title("Obstacles")]
		public Array2DGrid Obstacles;

		[Serializable]
		public class PersonGroupSettings
		{
			[ValidateInput(nameof(ValidatePersonTypes)), ListDrawerSettings(AlwaysExpanded = true)]
			public List<PersonColor> PersonTypes = new List<PersonColor>();

			[Serializable]
			public struct PersonColor
			{
				[GUIColor("$GetColor")]
				public PersonType PersonType;

				private Color GetColor
				{
					get
					{
						var color = PersonType switch
						{
							PersonType.Blue => Color.blue,
							PersonType.Green => Color.green,
							PersonType.Orange => new Color(1f, 0.5f, 0),
							PersonType.Pink => Color.magenta,
							PersonType.Purple => new Color(.7f, .25f, 1f),
							PersonType.Red => Color.red,
							PersonType.Yellow => Color.yellow,
							PersonType.None => Color.white,
							_ => throw new ArgumentOutOfRangeException()
						};

						return color;
					}
				}
			}

			private TriValidationResult ValidatePersonTypes()
			{
				if (PersonTypes.Count > PersonGroup.MAX_PERSON_COUNT)
					return TriValidationResult.Error($"Max {PersonGroup.MAX_PERSON_COUNT}")
						.WithFix(() => PersonTypes.RemoveRange(PersonGroup.MAX_PERSON_COUNT, PersonTypes.Count - PersonGroup.MAX_PERSON_COUNT), $"Set to {PersonGroup.MAX_PERSON_COUNT}");
				if (!PersonTypes.Count.Equals(PersonGroup.MAX_PERSON_COUNT)) return TriValidationResult.Valid;
				var allSame = true;
				for (var i = 1; i < PersonTypes.Count; i++)
				{
					if (PersonTypes[i].PersonType.Equals(PersonTypes[i - 1].PersonType)) continue;
					allSame = false;
					break;
				}

				return allSame ? TriValidationResult.Error("All Colors can't be same!") : TriValidationResult.Valid;
			}
		}

		[Serializable]
		public class GoalSettings
		{
			[TableList(Draggable = true)]
			public Goal[] Goals;
		}

		[Serializable]
		public class Goal
		{
			public PersonGroupSettings.PersonColor GoalColor;
			[RangeStep(6, 18, 6)]
			public int Count = 18;
		}

		private void OnEnable()
		{
			personCounts = new List<PersonCount>();
			CalculateCount();
		}

		private void CalculateCount()
		{
			personCounts.Clear();
			foreach (var personGroupSetting in BoltPacks)
			{
				foreach (var personColor in personGroupSetting.PersonTypes)
				{
					var found = false;
					foreach (var personCount in personCounts.Where(personCount => personCount.PersonType == personColor.PersonType))
					{
						personCount.Count++;
						found = true;
					}

					if (!found)
					{
						personCounts.Add(new PersonCount(personColor.PersonType, 1));
					}
				}
			}
		}
	}
}