using System;
using System.Collections.Generic;
using Fiber.Managers;
using Fiber.Utilities;
using Fiber.Utilities.Extensions;
using GamePlay.People;
using ScriptableObjects;
using UnityEngine;

namespace DeckSystem
{
	public class Deck : Singleton<Deck>
	{
		[SerializeField] private int slotCount = 3;

		[Header("References")]
		[SerializeField] private DeckSlot deckSlotPrefab;
		[SerializeField] private PersonGroup personGroupPrefab;
		[SerializeField] private Transform spawnPoint;

		private readonly List<DeckSlot> deckSlots = new List<DeckSlot>();
		private readonly List<PersonGroup> personGroups = new List<PersonGroup>();
		private readonly List<PersonGroup> personGroupsInDeck = new List<PersonGroup>();

		private LevelDataSO.PersonGroupSettings[] currentPersonGroupSettings;

		public List<DeckSlot> DeckSlots => deckSlots;

		private void OnEnable()
		{
			LevelManager.OnLevelLoad += Setup;
			LevelManager.OnLevelStart += Spawn;
			PersonGroup.OnPlace += OnPersonGroupPlaced;
		}

		private void OnDisable()
		{
			LevelManager.OnLevelLoad -= Setup;
			LevelManager.OnLevelStart -= Spawn;
			PersonGroup.OnPlace -= OnPersonGroupPlaced;
		}

		private void Setup()
		{
			var offset = slotCount * deckSlotPrefab.Size / 2f - deckSlotPrefab.Size / 2f;
			for (int i = 0; i < slotCount; i++)
			{
				var slot = Instantiate(deckSlotPrefab, transform);
				slot.transform.localPosition = new Vector3(i * deckSlotPrefab.Size - offset, 0, 0);
				deckSlots.Add(slot);
			}

			Init();
		}

		public void Init()
		{
			var currentLevelData = LevelManager.Instance.CurrentLevelData.BoltPacks;
			currentPersonGroupSettings = new LevelDataSO.PersonGroupSettings[currentLevelData.Length];
			Array.Copy(currentLevelData, currentPersonGroupSettings, currentLevelData.Length);

			SetupPersonGroups();
		}

		private void SetupPersonGroups(bool randomize = false)
		{
			foreach (var setting in currentPersonGroupSettings)
			{
				var personGroup = Instantiate(personGroupPrefab, transform);
				personGroup.Init(setting.PersonTypes);
				personGroup.gameObject.SetActive(false);
				personGroups.Add(personGroup);
			}

			if (randomize)
				personGroups.Shuffle();
		}

		public void Spawn()
		{
			for (int i = 0; i < slotCount; i++)
			{
				if (personGroups.Count <= 0)
					SetupPersonGroups(true);

				var spawnedBoltPack = personGroups[0];
				personGroups.RemoveAt(0);
				personGroupsInDeck.Add(spawnedBoltPack);

				spawnedBoltPack.gameObject.SetActive(true);
				spawnedBoltPack.Setup(deckSlots[i], i * .1f);
				spawnedBoltPack.transform.position = spawnPoint.position;
			}
		}

		private void OnPersonGroupPlaced(PersonGroup personGroup)
		{
			personGroupsInDeck.Remove(personGroup);

			if (personGroupsInDeck.Count.Equals(0))
			{
				Spawn();
			}
		}
	}
}