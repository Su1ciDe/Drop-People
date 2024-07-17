using System.Collections.Generic;
using Fiber.Managers;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class StageUI : MonoBehaviour
	{
		[SerializeField] private StageCircleUI stageCircleUI;
		[SerializeField] private Image stageDash;

		[Space]
		[SerializeField] private RectTransform panel;

		private readonly List<StageCircleUI> stages = new List<StageCircleUI>();

		private void OnEnable()
		{
			LevelManager.OnLevelLoad += OnLevelLoaded;
			StageManager.OnStageStarted += OnStageStarted;
			StageManager.OnStageCompleted += OnStageCompleted;
		}

		private void OnDisable()
		{
			StageManager.OnStageStarted -= OnStageStarted;
			StageManager.OnStageCompleted -= OnStageCompleted;
		}

		private void Init()
		{
			for (int i = 0; i < StageManager.Instance.StageCount; i++)
			{
				var circle = Instantiate(stageCircleUI, panel);
				circle.Setup(i);
				if (i + 1 < StageManager.Instance.StageCount)
					Instantiate(stageDash, panel);

				stages.Add(circle);
			}
		}

		private void OnStageStarted(int stageNo)
		{
			if (StageManager.Instance.StageCount.Equals(1)) return;

			if (stages.Count == 0)
				Init();

			stages[stageNo].SetCurrentStage();
		}

		private void OnStageCompleted(int stageNo)
		{
			if (StageManager.Instance.StageCount.Equals(1)) return;

			stages[stageNo].CompleteStage();
		}

		private void OnLevelLoaded()
		{
			foreach (Transform child in panel)
				Destroy(child.gameObject);

			stages.Clear();
		}
	}
}