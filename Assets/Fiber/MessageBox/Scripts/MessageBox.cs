using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fiber.MessageSystem
{
	/// <summary>
	/// Displays a message window, also known as a dialog box, which presents a message to the player.
	/// </summary>
	public class MessageBox : MonoBehaviour
	{
		public static MessageBox Instance;

		[SerializeField] private bool closeWhenClickedOut = true;
		[Space]
		[SerializeField] private RectTransform messageBoxPanel;
		[SerializeField] private TMP_Text txtMessage;
		[SerializeField] private TMP_Text txtTitle;
		[Header("Icons")]
		[SerializeField] private Image imgInformation;
		[SerializeField] private Image imgQuestion;
		[SerializeField] private Image imgWarning;
		[SerializeField] private Image imgError;
		[Header("Buttons")]
		[SerializeField] private Button btnOk;
		[SerializeField] private Button btnYes;
		[SerializeField] private Button btnNo;
		[SerializeField] private Button btnCancel;
		[SerializeField] private Button btnBackgroundButton;
		[Header("Animations")]
		[SerializeField] private float animationDuration = .25f;
		[SerializeField] private Ease showEase = Ease.Linear;
		[SerializeField] private Ease hideEase = Ease.Linear;

		/// <summary>
		/// Specifies constants defining which buttons to display on a MessageBox.
		/// </summary>
		public enum MessageBoxButtons
		{
			Ok,
			OkCancel,
			YesNo,
			YesNoCancel
		}

		private MessageBoxButtons messageBoxButtons = MessageBoxButtons.Ok;

		/// <summary>
		/// Specifies constants defining which information to display.
		/// </summary>
		public enum MessageBoxType
		{
			None,
			Information,
			Question,
			Warning,
			Error
		}

		private MessageBoxType messageBoxType = MessageBoxType.None;

		private event Action OnTrue, OnFalse, OnCancel;

		private void Awake()
		{
			Instance = this;

			btnYes.onClick.AddListener(OnTrueButtonClicked);
			btnOk.onClick.AddListener(OnTrueButtonClicked);
			btnNo.onClick.AddListener(OnFalseButtonClicked);
			btnCancel.onClick.AddListener(OnCancelButtonClicked);
			if (closeWhenClickedOut)
				btnBackgroundButton.onClick.AddListener(OnCancelButtonClicked);
		}

		/// <summary>
		/// Displays a message box
		/// </summary>
		/// <param name="message">The text to display in the message box</param>
		/// <param name="onTrue">The Action when MessageBox returns true</param>
		/// <param name="onFalse">The Action when MessageBox returns false</param>
		/// <param name="onCancel">The Action when cancel button pressed</param>
		public void Show(string message, Action onTrue = null, Action onFalse = null, Action onCancel = null)
		{
			Show(message, "", MessageBoxButtons.Ok, MessageBoxType.None, onTrue, onFalse, onCancel);
		}

		/// <summary>
		/// Displays a message box
		/// </summary>
		/// <param name="message">The text to display in the message box</param>
		/// <param name="title">The text to display in the title of the message box</param>
		/// <param name="onTrue">The Action when MessageBox returns true</param>
		/// <param name="onFalse">The Action when MessageBox returns false</param>
		/// <param name="onCancel">The Action when cancel button pressed</param>
		public void Show(string message, string title, Action onTrue = null, Action onFalse = null, Action onCancel = null)
		{
			Show(message, title, MessageBoxButtons.Ok, MessageBoxType.None, onTrue, onFalse, onCancel);
		}

		/// <summary>
		/// Displays a message box
		/// </summary>
		/// <param name="message">The text to display in the message box</param>
		/// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box</param>
		/// <param name="onTrue">The Action when MessageBox returns true</param>
		/// <param name="onFalse">The Action when MessageBox returns false</param>
		/// <param name="onCancel">The Action when cancel button pressed</param>
		public void Show(string message, MessageBoxButtons buttons, Action onTrue = null, Action onFalse = null, Action onCancel = null)
		{
			Show(message, "", buttons, MessageBoxType.None, onTrue, onFalse, onCancel);
		}

		/// <summary>
		/// Displays a message box
		/// </summary>
		/// <param name="message">The text to display in the message box</param>
		/// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box</param>
		/// <param name="type">One of the MessageBoxType values that specifies which display and association options will be used for the message box</param>
		/// <param name="onTrue">The Action when MessageBox returns true</param>
		/// <param name="onFalse">The Action when MessageBox returns false</param>
		/// <param name="onCancel">The Action when cancel button pressed</param>
		public void Show(string message, MessageBoxButtons buttons, MessageBoxType type, Action onTrue = null, Action onFalse = null, Action onCancel = null)
		{
			Show(message, "", buttons, type, onTrue, onFalse, onCancel);
		}

		/// <summary>
		/// Displays a message box
		/// </summary>
		/// <param name="message">The text to display in the message box</param>
		/// <param name="title">The text to display in the title of the message box</param>
		/// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box</param>
		/// <param name="onTrue">The Action when MessageBox returns true</param>
		/// <param name="onFalse">The Action when MessageBox returns false</param>
		/// <param name="onCancel">The Action when cancel button pressed</param>
		public void Show(string message, string title, MessageBoxButtons buttons, Action onTrue = null, Action onFalse = null, Action onCancel = null)
		{
			Show(message, title, buttons, MessageBoxType.None, onTrue, onFalse, onCancel);
		}

		/// <summary>
		/// Displays a message box
		/// </summary>
		/// <param name="message">The text to display in the message box</param>
		/// <param name="title">The text to display in the title of the message box</param>
		/// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box</param>
		/// <param name="type">One of the MessageBoxType values that specifies which display and association options will be used for the message box</param>
		/// <param name="onTrue">The Action when MessageBox returns true</param>
		/// <param name="onFalse">The Action when MessageBox returns false</param>
		/// <param name="onCancel">The Action when cancel button pressed</param>
		public void Show(string message, string title, MessageBoxButtons buttons, MessageBoxType type, Action onTrue = null, Action onFalse = null, Action onCancel = null)
		{
			txtMessage.SetText(message);
			txtTitle.SetText(title);
			SetupButtons(buttons);
			SetupType(type);

			OnTrue = onTrue;
			OnFalse = onFalse;
			OnCancel = onCancel;

			Show();
		}

		private void SetupButtons(MessageBoxButtons buttons)
		{
			btnOk.gameObject.SetActive(false);
			btnYes.gameObject.SetActive(false);
			btnNo.gameObject.SetActive(false);
			btnCancel.gameObject.SetActive(false);

			switch (buttons)
			{
				case MessageBoxButtons.Ok:
					btnOk.gameObject.SetActive(true);
					break;
				case MessageBoxButtons.OkCancel:
					btnOk.gameObject.SetActive(true);
					btnCancel.gameObject.SetActive(true);
					break;
				case MessageBoxButtons.YesNo:
					btnYes.gameObject.SetActive(true);
					btnNo.gameObject.SetActive(true);
					break;
				case MessageBoxButtons.YesNoCancel:
					btnYes.gameObject.SetActive(true);
					btnNo.gameObject.SetActive(true);
					btnCancel.gameObject.SetActive(true);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(buttons), buttons, null);
			}
		}

		private void SetupType(MessageBoxType type)
		{
			imgInformation.gameObject.SetActive(false);
			imgQuestion.gameObject.SetActive(false);
			imgWarning.gameObject.SetActive(false);
			imgError.gameObject.SetActive(false);

			switch (type)
			{
				case MessageBoxType.None:
					break;
				case MessageBoxType.Information:
					imgInformation.gameObject.SetActive(true);
					break;
				case MessageBoxType.Question:
					imgQuestion.gameObject.SetActive(true);
					break;
				case MessageBoxType.Warning:
					imgWarning.gameObject.SetActive(true);
					break;
				case MessageBoxType.Error:
					imgError.gameObject.SetActive(true);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(type), type, null);
			}
		}

		private void OnTrueButtonClicked()
		{
			Hide();
			OnTrue?.Invoke();
		}

		private void OnFalseButtonClicked()
		{
			Hide();
			OnFalse?.Invoke();
		}

		private void OnCancelButtonClicked()
		{
			Hide();
			OnCancel?.Invoke();
		}

		private void Show()
		{
			btnBackgroundButton.gameObject.SetActive(true);

			messageBoxPanel.gameObject.SetActive(true);
			messageBoxPanel.localScale = Vector3.zero;
			messageBoxPanel.DOComplete();
			messageBoxPanel.DOScale(1, animationDuration).SetEase(showEase);
		}

		private void Hide()
		{
			btnBackgroundButton.gameObject.SetActive(false);

			messageBoxPanel.localScale = Vector3.one;
			messageBoxPanel.DOComplete();
			messageBoxPanel.DOScale(0, animationDuration).SetEase(hideEase).OnComplete(() => messageBoxPanel.gameObject.SetActive(false));
		}
	}
}