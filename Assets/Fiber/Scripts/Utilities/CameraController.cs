using Cinemachine;
using TriInspector;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Fiber.Utilities
{
	public class CameraController : Singleton<CameraController>
	{
		public CinemachineVirtualCamera CurrentCamera { get; private set; }

		[Title("iPhone")]
		[SerializeField] private CinemachineVirtualCamera iphoneCam;
		[SerializeField] [Min(0)] private float iphoneShadowDistance = 100;
		[Title("iPad")]
		[SerializeField] private CinemachineVirtualCamera ipadCam;
		[SerializeField] [Min(0)] private float ipadShadowDistance = 120;

		private void Awake()
		{
			CurrentCamera = iphoneCam;

			AdjustByScreenRatio();
		}

		private void OnValidate()
		{
			ChangeShadowDistance(iphoneShadowDistance);
		}

		private void AdjustByScreenRatio()
		{
			var ratio = (float)Screen.width / Screen.height;
			if (ratio < 1.6f) // iPhone
			{
				iphoneCam.gameObject.SetActive(true);
				ipadCam?.gameObject.SetActive(false);
				CurrentCamera = iphoneCam;

				ChangeShadowDistance(iphoneShadowDistance);
			}
			else if (ipadCam)
			{
				iphoneCam.gameObject.SetActive(false);
				ipadCam.gameObject.SetActive(true);
				CurrentCamera = ipadCam;

				ChangeShadowDistance(ipadShadowDistance);
			}
		}

		private void ChangeShadowDistance(float distance)
		{
			QualitySettings.shadowDistance = distance;
			var urp = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;
			urp.shadowDistance = distance;
		}
	}
}