using Fiber.UI;
using Fiber.Managers;
using Fiber.Utilities;
using Lofelt.NiceVibrations;
using DG.Tweening;
using UnityEngine;
using TMPro;

namespace Fiber.CurrencySystem
{
	public abstract class CurrencyUI : MonoBehaviour
	{
		[SerializeField] protected RectTransform target;
		[SerializeField] protected TMP_Text txt_Currency;
		[SerializeField] protected string currencyPoolName;
		[SerializeField] protected string currencyFloatingPoolName;

		private Currency currency;
		private long currentCurrencyAmount;

		private const float SPAWN_DURATION = .4F;
		private const float MOVE_DURATION = 1;
		private const float DELAY = .05F;

		protected virtual void OnEnable()
		{
			currency.OnAmountAdded += AmountAdded;
			currency.OnAmountAddedFloating += AmountAddedFloating;
			currency.OnAmountSpent += AmountSpent;
			currency.OnNotEnoughCurrency += NotEnoughCurrency;
		}

		protected virtual void OnDisable()
		{
			currency.OnAmountAdded -= AmountAdded;
			currency.OnAmountAddedFloating -= AmountAddedFloating;
			currency.OnAmountSpent -= AmountSpent;
			currency.OnNotEnoughCurrency -= NotEnoughCurrency;
		}

		protected virtual void OnDestroy()
		{
			DOTween.Complete("icon");
			DOTween.Complete("icon-spend");
			DOTween.Complete("icon-floating");
			target.DOComplete();
		}

		public virtual void Init(Currency _currency)
		{
			currency = _currency;
			currentCurrencyAmount = currency.Amount;

			SetCurrencyText(currentCurrencyAmount);
		}

		protected void SetCurrencyText(long amount)
		{
			txt_Currency.SetText(amount.ToString());
		}

		protected virtual void AmountAdded(long amount, Vector3? position = null, bool isWorldPosition = true)
		{
			if (position is null)
			{
				currentCurrencyAmount += amount;
				SetCurrencyText(Mathf.CeilToInt(currentCurrencyAmount));
			}
			else
			{
				if (isWorldPosition)
				{
					position = Helper.MainCamera.WorldToScreenPoint((Vector3)position);
				}

				var tempCurrency = currentCurrencyAmount + amount;
				var tempCurrentCurrency = currentCurrencyAmount;

				int count = Mathf.Clamp((int)amount, 2, 10);
				for (int i = 0; i < count; i++)
				{
					var icon = ObjectPooler.Instance.Spawn(currencyPoolName, (Vector3)position);
					icon.transform.SetParent(transform);
					float i1 = i;
					icon.transform.DOMove((Vector3)position + 100 * (Vector3)Random.insideUnitCircle, SPAWN_DURATION).SetDelay(i * DELAY).SetEase(Ease.InOutSine).SetId("icon").OnComplete(() =>
					{
						icon.transform.DOMove(target.position, MOVE_DURATION).SetEase(Ease.InBack).OnComplete(() =>
						{
							tempCurrency = currentCurrencyAmount + amount;

							target.DOComplete();
							target.DOPunchScale(.9f * Vector3.one, .2f, 2, .5f);
							SetCurrencyText(Mathf.CeilToInt(Mathf.Lerp(tempCurrentCurrency, tempCurrency, i1 / (count - 1))));

							ObjectPooler.Instance.Release(icon, currencyPoolName);
							HapticManager.Instance.PlayHaptic(.6f, 0);
						});
					});
				}

				DOVirtual.DelayedCall(count * DELAY + SPAWN_DURATION + MOVE_DURATION, () => currentCurrencyAmount = tempCurrency);
			}
		}

		protected virtual void AmountAddedFloating(long amount, Vector3 position, float height)
		{
			long tempCurrency = currentCurrencyAmount + amount;

			var icon = ObjectPooler.Instance.Spawn(currencyFloatingPoolName, position).GetComponent<FloatingText>();
			icon.Setup(amount);
			icon.Float(position + height * Vector3.up, currencyFloatingPoolName);

			txt_Currency.SetText(Mathf.CeilToInt(tempCurrency).ToString());
			target.DOComplete();
			target.DOPunchScale(Vector3.one * .9f, .2f, 2, .5f);

			currentCurrencyAmount = tempCurrency;
		}

		protected virtual void AmountSpent(long amount)
		{
			DOTween.Complete("icon-spend");
			long tempCurrency = currentCurrencyAmount - amount;
			DOTween.To(() => currentCurrencyAmount, x => currentCurrencyAmount = x, tempCurrency, 1).SetEase(Ease.OutCubic)
				.OnUpdate(() => txt_Currency.SetText(Mathf.CeilToInt(currentCurrencyAmount).ToString())).OnComplete(() =>
				{
					currentCurrencyAmount = tempCurrency;
					txt_Currency.SetText(currentCurrencyAmount.ToString());
				}).SetId("icon-spend");
		}

		protected virtual void NotEnoughCurrency()
		{
			HapticManager.Instance.PlayHaptic(HapticPatterns.PresetType.Warning);

			txt_Currency.DOComplete();
			txt_Currency.transform.DOComplete();
			txt_Currency.transform.DOScale(1.5f, .1f).SetLoops(2, LoopType.Yoyo);
			txt_Currency.DOColor(Color.red, .1f).SetLoops(2, LoopType.Yoyo);
		}
	}
}