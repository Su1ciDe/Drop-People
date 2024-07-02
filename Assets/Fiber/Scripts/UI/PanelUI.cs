using UnityEngine;

namespace Fiber.UI
{
	public abstract class PanelUI : MonoBehaviour
	{
		public virtual void Open()
		{
			gameObject.SetActive(true);
		}

		public virtual void Close()
		{
			gameObject.SetActive(false);
		}
	}
}