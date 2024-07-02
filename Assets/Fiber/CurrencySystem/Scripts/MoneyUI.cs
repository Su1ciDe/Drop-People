namespace Fiber.CurrencySystem
{
	public class MoneyUI : CurrencyUI
	{
		protected override void OnEnable()
		{
			Init(CurrencyManager.Money);
			
			base.OnEnable();
		}
	}
}