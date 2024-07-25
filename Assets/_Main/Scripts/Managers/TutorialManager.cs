using System.Collections;
using DeckSystem;
using Fiber.Managers;
using Fiber.UI;
using Fiber.Utilities;
using GamePlay.People;
using UnityEngine;
using Grid = GridSystem.Grid;

namespace Managers
{
	public class TutorialManager : MonoBehaviour
	{
		private TutorialUI tutorialUI => TutorialUI.Instance;

		private void OnEnable()
		{
			LevelManager.OnLevelStart += OnLevelStarted;
			LevelManager.OnLevelUnload += OnLevelUnloaded;
		}

		private void OnDisable()
		{
			LevelManager.OnLevelStart -= OnLevelStarted;
			LevelManager.OnLevelUnload -= OnLevelUnloaded;

			Unsub();
		}

		private void OnLevelStarted()
		{
			if (LevelManager.Instance.LevelNo.Equals(1))
			{
				StartCoroutine(Level1Tutorial());
			}
		}

		private void OnLevelUnloaded()
		{
			Unsub();
		}

		private void Unsub()
		{
			StopAllCoroutines();

			PersonGroup.OnPlace -= OnFirstPersonGroupPlaced;
			PersonGroup.OnPlace -= OnSecondPersonGroupPlaced;
		}

		#region Level 1 Tutorial

		private IEnumerator Level1Tutorial()
		{
			yield return new WaitForSeconds(1);

			var deckSlot = Deck.Instance.GetFirstGroupInDeck();
			var cell = Grid.Instance.GetFirstEmptyCell();

			tutorialUI.ShowSwipe(deckSlot.transform.position, cell.transform.position, Helper.MainCamera);

			PersonGroup.OnPlace += OnFirstPersonGroupPlaced;
		}

		private void OnFirstPersonGroupPlaced(PersonGroup personGroup)
		{
			PersonGroup.OnPlace -= OnFirstPersonGroupPlaced;

			tutorialUI.HideHand();

			StartCoroutine(WaitForSecondGroup());
		}

		private IEnumerator WaitForSecondGroup()
		{
			yield return new WaitForSeconds(1);

			var deckSlot = Deck.Instance.GetFirstGroupInDeck();
			var cell = Grid.Instance.GetFirstEmptyCell();

			tutorialUI.ShowSwipe(deckSlot.transform.position, cell.transform.position, Helper.MainCamera);

			tutorialUI.ShowText("Complete Groups!");

			PersonGroup.OnPlace += OnSecondPersonGroupPlaced;
		}

		private void OnSecondPersonGroupPlaced(PersonGroup personGroup)
		{
			PersonGroup.OnPlace -= OnSecondPersonGroupPlaced;

			tutorialUI.HideHand();
			tutorialUI.HideText();
		}

		#endregion
	}
}