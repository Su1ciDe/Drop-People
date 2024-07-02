using Fiber.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fiber.UI
{
	public class WinPanel : PanelUI
	{
		[SerializeField] private TMP_Text txtLevelNo;
		[SerializeField] private Button btnContinue;

		private void Awake()
		{
			btnContinue.onClick.AddListener(Win);
		}

		private void Win()
		{
			LevelManager.Instance.LoadNextLevel();
			Close();
		}

		private void SetLevelNo()
		{
			txtLevelNo.SetText("LEVEL " + LevelManager.Instance.LevelNo.ToString());
		}

		public override void Open()
		{
			SetLevelNo();
			base.Open();
		}
	}
}