using Fiber.Utilities;
using UnityEngine;

namespace Fiber.CurrencySystem
{
	/// <summary>
	/// Soft currency
	/// </summary>
	public class Money : Currency
	{
		public override long Amount
		{
			get => PlayerPrefs.GetInt(PlayerPrefsNames.MONEY, 0);
			set => PlayerPrefs.SetInt(PlayerPrefsNames.MONEY, (int)value);
		}
	}
}