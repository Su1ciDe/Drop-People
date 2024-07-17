using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class StageCircleUI : MonoBehaviour
	{
		[SerializeField] private Image imgCurrentStage;
		[SerializeField] private Image imgCompleted;
		[SerializeField] private TMP_Text txtStageNumber;

		public void Setup(int stageNo)
		{
			txtStageNumber.SetText((stageNo + 1).ToString());
		}

		public void SetCurrentStage()
		{
			imgCurrentStage.gameObject.SetActive(true);
		}

		public void CompleteStage()
		{
			imgCurrentStage.gameObject.SetActive(false);
			imgCompleted.gameObject.SetActive(true);
			imgCompleted.rectTransform.DOPunchScale(Vector3.one, .35f, 2);
		}
	}
}