using DG.Tweening;
using Fiber.Utilities;
using TMPro;
using UnityEngine;

namespace Fiber.UI
{
	public class FloatingText : MonoBehaviour
	{
		[SerializeField] private TMP_Text txtAmount;
		[SerializeField] private CanvasGroup canvasGroup;

		public void Setup(long amount)
		{
			txtAmount.SetText(amount.ToString());
			canvasGroup.alpha = 1;
		}

		public void Float(Vector3 position, string currencyFloatingPoolName)
		{
			var t = transform;
			t.position = position;

			var rotation = Helper.MainCamera.transform.rotation;
			t.LookAt(t.position + rotation * Vector3.forward, rotation * Vector3.up);

			canvasGroup.DOFade(0, 1).SetDelay(1).SetEase(Ease.OutCubic);
			t.DOMoveY(1, 1).SetRelative(true).SetDelay(1).SetEase(Ease.OutCubic).OnComplete(() => ObjectPooler.Instance.Release(gameObject, currencyFloatingPoolName));
		}
	}
}