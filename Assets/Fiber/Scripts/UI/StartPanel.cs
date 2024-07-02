using Fiber.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Fiber.UI
{
	public class StartPanel : PanelUI
	{
		[SerializeField] private Button btnStart;

		private void Awake()
		{
			btnStart.onClick.AddListener(StartLevel);
		}

		private void StartLevel()
		{
			LevelManager.Instance.StartLevel();
			Close();
		}
	}
}