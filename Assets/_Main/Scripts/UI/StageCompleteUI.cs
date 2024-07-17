using DG.Tweening;
using Fiber.UI;
using Managers;
using UnityEngine;

namespace UI
{
	public class StageCompleteUI : PanelUI
	{
		[SerializeField] private RectTransform completePanel;

		private const float MOVE_DURATION = .5f;
		private const float ROTATION_DURATION = .35f;

		public override void Open()
		{
			base.Open();

			var seq = DOTween.Sequence();

			completePanel.position = new Vector3(-2000, completePanel.position.y, completePanel.position.z);
			completePanel.eulerAngles = new Vector3(0, 0, 30);

			seq.Append(completePanel.DOLocalMoveX(0, MOVE_DURATION));
			seq.Append(completePanel.DORotate(Vector3.zero, ROTATION_DURATION).SetEase(Ease.OutBack));
			seq.AppendInterval(1);
			seq.AppendCallback(StartNextStage);
			seq.Append(completePanel.DORotate(new Vector3(0, 0, 30), ROTATION_DURATION).SetEase(Ease.InBack));
			seq.Append(completePanel.DOLocalMoveX(2000, MOVE_DURATION));
		}

		private void StartNextStage()
		{
			StageManager.Instance.StartNextStage();
		}
	}
}