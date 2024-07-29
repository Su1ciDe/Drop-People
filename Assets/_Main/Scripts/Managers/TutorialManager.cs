using System.Collections;
using DeckSystem;
using Fiber.UI;
using Fiber.Managers;
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

		private void OnLevelUnloaded()
		{
			Unsub();
		}

		private void Unsub()
		{
			StopAllCoroutines();

			PersonGroup.OnPlace -= OnFirstPersonGroupPlaced;
			PersonGroup.OnPlace -= OnSecondPersonGroupPlaced;
			PersonGroup.OnPlace -= OnFirstPersonGroupPlaced_BreakableTutorial;

			if (tutorialUI)
			{
				tutorialUI.HideText();
				tutorialUI.HideHand();
			}
		}

		private void OnLevelStarted()
		{
			if (LevelManager.Instance.LevelNo.Equals(1))
			{
				StartCoroutine(Level1Tutorial());
			}

			if (LevelManager.Instance.LevelNo.Equals(12))
			{
				StartCoroutine(BreakableTutorial());
			}
		}

		#region Level 1 Tutorial

		private IEnumerator Level1Tutorial()
		{
			yield return new WaitForSeconds(0.5f);

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
			yield return new WaitForSeconds(0.5f);

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

		#region Breakable Tutorial

		private IEnumerator BreakableTutorial()
		{
			yield return new WaitForSeconds(0.5f);

			var deckSlot = Deck.Instance.GetFirstGroupInDeck();
			var cell = Grid.Instance.GetFirstEmptyCell(true);

			tutorialUI.ShowSwipe(deckSlot.transform.position, cell.transform.position, Helper.MainCamera);
			tutorialUI.ShowText("Make a match to break obstacles!");

			PersonGroup.OnPlace += OnFirstPersonGroupPlaced_BreakableTutorial;
		}

		private void OnFirstPersonGroupPlaced_BreakableTutorial(PersonGroup personGroup)
		{
			PersonGroup.OnPlace -= OnFirstPersonGroupPlaced_BreakableTutorial;

			tutorialUI.HideHand();

			StartCoroutine(WaitForSecondGroup_BreakableTutorial());
		}

		private IEnumerator WaitForSecondGroup_BreakableTutorial()
		{
			yield return new WaitForSeconds(0.5f);

			var deckSlot = Deck.Instance.GetFirstGroupInDeck();
			var cell = Grid.Instance.GetFirstEmptyCell(true);

			tutorialUI.ShowSwipe(deckSlot.transform.position, cell.transform.position, Helper.MainCamera);

			PersonGroup.OnPlace += OnSecondPersonGroupPlaced_BreakableTutorial;
		}

		private void OnSecondPersonGroupPlaced_BreakableTutorial(PersonGroup personGroup)
		{
			PersonGroup.OnPlace -= OnSecondPersonGroupPlaced_BreakableTutorial;

			tutorialUI.HideHand();
			tutorialUI.HideText();
		}

		#endregion
	}
}