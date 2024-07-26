using DG.Tweening;
using Fiber.Managers;
using Fiber.Utilities;
using Fiber.AudioSystem;
using GamePlay.People;
using Interfaces;
using Lean.Touch;
using UnityEngine;
using UnityEngine.Events;

namespace GamePlay.Player
{
	public class PlayerInputs : MonoBehaviour, IInputs
	{
		public bool CanInput { get; set; }

		public PersonGroup SelectedPersonGroup { get; private set; }

		[SerializeField] private Vector3 offset;
		[SerializeField] private float rotationDamping = 10;
		[SerializeField] private float rotationClamp = 25;
		[Space]
		[SerializeField] private LayerMask touchLayers;

		private LayerMask inputPlaneLayer;

		public event UnityAction<Vector3> OnDown;
		public event UnityAction<Vector3> OnMove;
		public event UnityAction<Vector3> OnUp;

		private void Awake()
		{
			Input.multiTouchEnabled = false;
			
			LeanTouch.OnFingerDown += OnFingerDown;
			LeanTouch.OnFingerUpdate += OnFingerUpdate;
			LeanTouch.OnFingerUp += OnFingerUp;

			LevelManager.OnLevelStart += OnLevelStarted;
			LevelManager.OnLevelWin += OnLevelWon;
			LevelManager.OnLevelLose += OnLevelLost;

			inputPlaneLayer = LayerMask.GetMask("InputPlane");
		}

		private void OnDestroy()
		{
			LeanTouch.OnFingerDown -= OnFingerDown;
			LeanTouch.OnFingerUpdate -= OnFingerUpdate;
			LeanTouch.OnFingerUp -= OnFingerUp;

			LevelManager.OnLevelStart -= OnLevelStarted;
			LevelManager.OnLevelStart -= OnLevelWon;
			LevelManager.OnLevelStart -= OnLevelLost;
		}

		private void OnFingerDown(LeanFinger finger)
		{
			if (SelectedPersonGroup) return;
			if (!CanInput) return;
			if (finger.IsOverGui) return;

			var ray = finger.GetRay(Helper.MainCamera);
			if (Physics.Raycast(ray, out var hit, 100, touchLayers))
			{
				if (hit.rigidbody && hit.rigidbody.TryGetComponent(out PersonGroup group) && group.CanMove)
				{
					HapticManager.Instance.PlayHaptic(0.5f, 0.5f);
					AudioManager.Instance.PlayAudio(AudioName.Plop1);

					group.transform.DOKill();
					SelectedPersonGroup = group;
					SelectedPersonGroup.OnPickUp();

					OnDown?.Invoke(hit.point);
				}
			}
		}

		private void OnFingerUpdate(LeanFinger finger)
		{
			if (!CanInput) return;
			if (finger.IsOverGui) return;
			if (!SelectedPersonGroup) return;
			if (!SelectedPersonGroup.CanMove) return;

			var ray = finger.GetRay(Helper.MainCamera);
			if (Physics.Raycast(ray, out var hit, 100, inputPlaneLayer))
			{
				var position = hit.point + offset;

				SelectedPersonGroup.Move(position);
				OnMove?.Invoke(position);
			}
		}

		private void OnFingerUp(LeanFinger finger)
		{
			if (!CanInput) return;
			if (finger.IsOverGui) return;
			if (!SelectedPersonGroup) return;
			if (!SelectedPersonGroup.CanMove) return;

			var ray = finger.GetRay(Helper.MainCamera);
			if (Physics.Raycast(ray, out var hit, 100, inputPlaneLayer))
			{
				AudioManager.Instance.PlayAudio(AudioName.Place);
				var position = hit.point + offset;
				SelectedPersonGroup.OnRelease();
				OnUp?.Invoke(position);
			}

			SelectedPersonGroup = null;
		}

		private void OnLevelStarted()
		{
			CanInput = true;
		}

		private void OnLevelWon()
		{
			CanInput = false;
		}

		private void OnLevelLost()
		{
			CanInput = false;
		}
	}
}