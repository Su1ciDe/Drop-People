using System;
using UnityEngine;

namespace Fiber.Utilities
{
	public class LookAtCamera : MonoBehaviour
	{
		private enum LookMode
		{
			/// <summary>
			/// Looks at the camera position
			/// </summary>
			LookAt,
			/// <summary>
			/// Looks at the inverted camera position 
			/// </summary>
			LookAtInverted,
			/// <summary>
			/// Looks at camera's forward
			/// </summary>
			CameraForward,
			/// <summary>
			/// Looks at inverted camera's forward
			/// </summary>
			CameraForwardInverted,
			/// <summary>
			/// Billboard
			/// </summary>
			Billboard,
		}

		[SerializeField] private LookMode lookMode;
		[SerializeField] private bool updateEveryFrame;

		private Camera mainCamera => Helper.MainCamera;

		private void Awake()
		{
			if (TryGetComponent(out Canvas canvas))
				canvas.worldCamera = mainCamera;

			if (!updateEveryFrame)
				Look();
		}

		private void LateUpdate()
		{
			if (!updateEveryFrame) return;
			Look();
		}

		private void Look()
		{
			switch (lookMode)
			{
				case LookMode.LookAt:
					transform.LookAt(mainCamera.transform);
					break;
				case LookMode.LookAtInverted:
					var dirFromCamera = transform.position - mainCamera.transform.position;
					transform.LookAt(transform.position + dirFromCamera);
					break;
				case LookMode.CameraForward:
					transform.forward = mainCamera.transform.forward;
					break;
				case LookMode.CameraForwardInverted:
					transform.forward = -mainCamera.transform.forward;
					break;
				case LookMode.Billboard:
					transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}