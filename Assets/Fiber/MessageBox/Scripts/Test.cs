using Fiber.MessageSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Fiber.MessageSystem
{
	public class Test : MonoBehaviour
	{
		[SerializeField] private string message;
		[SerializeField] private string title;
		[SerializeField] private MessageBox.MessageBoxButtons buttons;
		[SerializeField] private MessageBox.MessageBoxType type;

		[SerializeField]private Button button;

		private void Awake()
		{
			button.onClick.AddListener(OnClick);
		}

		private void OnClick()
		{
			MessageBox.Instance.Show(message, title, buttons, type, Yes, No, Cancel);
		}

		private void Yes()
		{
			button.image.color = Color.green;
			Debug.Log("yes");
		}

		private void No()
		{
			button.image.color = Color.red;

			Debug.Log("no");
		}

		private void Cancel()
		{
			button.image.color = Color.white;

			Debug.Log("cancel");
		}
	}
}