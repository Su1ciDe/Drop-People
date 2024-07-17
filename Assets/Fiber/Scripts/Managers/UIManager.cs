using Fiber.UI;
using Fiber.Utilities;
using Managers;
using TMPro;
using TriInspector;
using UI;
using UnityEngine;

namespace Fiber.Managers
{
	public class UIManager : SingletonInit<UIManager>
	{
		[SerializeField] private TextMeshProUGUI levelText;

		[Title("Panels")]
		[SerializeField] private StartPanel startPanel;
		[SerializeField] private WinPanel winPanel;
		[SerializeField] private LosePanel losePanel;
		[SerializeField] private SettingsUI settingsPanel;
		[SerializeField] private StageCompleteUI stageCompleteUI;
		[Space]
		[SerializeField] private GameObject inputPanel;
		public InGameUI InGameUI { get; private set; }

		protected override void Awake()
		{
			base.Awake();

			InGameUI = GetComponentInChildren<InGameUI>();
			//InGameUI.Hide();
		}

		private void OnEnable()
		{
			LevelManager.OnLevelUnload += OnLevelUnloaded;
			LevelManager.OnLevelLoad += OnLevelLoad;
			LevelManager.OnLevelStart += OnLevelStart;
			LevelManager.OnLevelWin += OnLevelWin;
			LevelManager.OnLevelLose += OnLevelLose;
			StageManager.OnStageCompleted += ShowStageCompletePanel;
		}

		private void OnDisable()
		{
			LevelManager.OnLevelUnload -= OnLevelUnloaded;
			LevelManager.OnLevelLoad -= OnLevelLoad;
			LevelManager.OnLevelStart -= OnLevelStart;
			LevelManager.OnLevelWin -= OnLevelWin;
			LevelManager.OnLevelLose -= OnLevelLose;
			StageManager.OnStageCompleted -= ShowStageCompletePanel;
		}

		private void ShowStageCompletePanel(int stageNo)
		{
			stageCompleteUI.Open();
		}

		private void ShowWinPanel()
		{
			winPanel.Open();
		}

		private void ShowLosePanel()
		{
			losePanel.Open();
		}

		private void HideWinPanel()
		{
			winPanel.Close();
		}

		private void HideLosePanel()
		{
			losePanel.Close();
		}

		private void HideStartPanel()
		{
			startPanel.Close();
		}

		public void ShowSettingsPanel()
		{
			settingsPanel.Open();
		}

		public void HideSettingsPanel()
		{
			settingsPanel.Close();
		}

		private void ShowInGameUI()
		{
			InGameUI.Show();
		}

		private void HideInGameUI()
		{
			InGameUI.Hide();
		}

		private void UpdateLevelText()
		{
			levelText.SetText(LevelManager.Instance.LevelNo.ToString());
		}

		private void EnableInput()
		{
			inputPanel.SetActive(true);
		}

		private void DisableInput()
		{
			inputPanel.SetActive(false);
		}

		private void OnLevelUnloaded()
		{
			HideWinPanel();
			HideLosePanel();
		}

		private void OnLevelLoad()
		{
			UpdateLevelText();
			startPanel.Open();
		}

		private void OnLevelStart()
		{
			UpdateLevelText();
			ShowInGameUI();
			HideStartPanel();
			EnableInput();
		}

		private void OnLevelWin()
		{
			ShowWinPanel();
			HideInGameUI();
			DisableInput();
		}

		private void OnLevelLose()
		{
			ShowLosePanel();
			HideInGameUI();
			DisableInput();
		}
	}
}