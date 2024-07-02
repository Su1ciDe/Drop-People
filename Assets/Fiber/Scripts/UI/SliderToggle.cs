using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Fiber.UI
{
	[AddComponentMenu("UI/SliderToggle", 31)]
	[RequireComponent(typeof(RectTransform))]
	public class SliderToggle : Selectable, IPointerClickHandler, ISubmitHandler, ICanvasElement
	{
		[Space]
		// Whether the toggle is on
		[Tooltip("Is the toggle currently on or off?")]
		[SerializeField] private bool m_IsOn;
		public bool isOn
		{
			get => m_IsOn;

			set { Set(value); }
		}

		public enum ToggleTransition
		{
			/// <summary>
			/// Show / hide the toggle instantly
			/// </summary>
			None,

			/// <summary>
			/// Fade the toggle in / out smoothly.
			/// </summary>
			Animated
		}

		/// <summary>
		/// Transition mode for the toggle.
		/// </summary>
		public ToggleTransition toggleTransition = ToggleTransition.Animated;

		[Serializable]
		public class ToggleEvent : UnityEvent<bool>
		{
		}

		/// <summary>
		/// Graphic the toggle should be working with.
		/// </summary>
		public Graphic handle;
		[SerializeField] private RectTransform onPoint;
		[SerializeField] private RectTransform offPoint;

		[Space]
		public ToggleEvent onValueChanged = new ToggleEvent();

		protected SliderToggle()
		{
		}

		/// <summary>
		/// Assume the correct visual state.
		/// </summary>
		protected override void Start()
		{
			PlayEffect(true);
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			PlayEffect(true);
		}

#if UNITY_EDITOR
		protected override void OnValidate()
		{
			base.OnValidate();

			if (!UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this) && !Application.isPlaying)
				CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
		}

#endif

		public virtual void Rebuild(CanvasUpdate executing)
		{
#if UNITY_EDITOR
			if (executing == CanvasUpdate.Prelayout)
				onValueChanged.Invoke(m_IsOn);
#endif
		}

		protected override void OnDidApplyAnimationProperties()
		{
			// Check if isOn has been changed by the animation.
			// Unfortunately there is no way to check if we don't have a graphic.
			if (handle != null)
			{
				bool oldValue = !Mathf.Approximately(handle.canvasRenderer.GetColor().a, 0);
				if (m_IsOn != oldValue)
				{
					m_IsOn = oldValue;
					Set(!oldValue);
				}
			}

			base.OnDidApplyAnimationProperties();
		}

		/// <summary>
		/// Set isOn without invoking onValueChanged callback.
		/// </summary>
		/// <param name="value">New Value for isOn.</param>
		public void SetIsOnWithoutNotify(bool value)
		{
			Set(value, false);
		}

		private void Set(bool value, bool sendCallback = true)
		{
			if (m_IsOn == value) return;

			// if we are in a group and set to true, do group logic
			m_IsOn = value;
			if (IsActive() && m_IsOn)
			{
				m_IsOn = true;
			}

			// Always send event when toggle is clicked, even if value didn't change
			// due to already active toggle in a toggle group being clicked.
			// Controls like Dropdown rely on this.
			// It's up to the user to ignore a selection being set to the same value it already was, if desired.
			PlayEffect(toggleTransition == ToggleTransition.None);

			if (sendCallback)
			{
				UISystemProfilerApi.AddMarker("Toggle.value", this);
				onValueChanged.Invoke(m_IsOn);
			}
		}

		/// <summary>
		/// Play the appropriate effect.
		/// </summary>
		private void PlayEffect(bool instant)
		{
			if (!handle) return;
			var duration = instant ? 0 : .2f;

#if UNITY_EDITOR
			if (!Application.isPlaying)
				handle.rectTransform.localPosition = m_IsOn ? onPoint.localPosition : offPoint.localPosition;
			else
#endif
			{
				handle.rectTransform.DOKill();
				var pos = m_IsOn ? onPoint.localPosition : offPoint.localPosition;
				handle.rectTransform.DOLocalMove(pos, duration).SetEase(Ease.InOutQuart).SetUpdate(true);
			}
		}

		private void InternalToggle()
		{
			if (!IsActive() || !IsInteractable()) return;

			isOn = !isOn;
		}

		/// <summary>
		/// React to clicks.
		/// </summary>
		public virtual void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left) return;

			InternalToggle();
		}

		public virtual void OnSubmit(BaseEventData eventData)
		{
			InternalToggle();
		}

		public void LayoutComplete()
		{
		}

		public void GraphicUpdateComplete()
		{
		}
	}
}