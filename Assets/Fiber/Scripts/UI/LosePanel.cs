using Fiber.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fiber.UI
{
	public class LosePanel : PanelUI
	{
		[SerializeField] private TMP_Text txtLevelNo;
		[SerializeField] private Button btnRetry;

		private void Awake()
		{
			btnRetry.onClick.AddListener(Retry);
		}

		private void Retry()
		{
			LevelManager.Instance.RetryLevel();
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