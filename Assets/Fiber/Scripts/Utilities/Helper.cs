using System.Globalization;
using Fiber.Utilities.Extensions;
using UnityEngine;

namespace Fiber.Utilities
{
	public static class Helper
	{
		public static Camera MainCamera { get; }

		static Helper()
		{
			MainCamera = Camera.main;
		}

		public enum CornerType
		{
			TopLeft,
			TopRight,
			BottomRight,
			BottomLeft,
		}

		/// <summary>
		/// Gives you the given corner position
		/// </summary>
		/// <param name="corner">Which corner</param>
		/// <returns>Position of the corner</returns>
		public static Vector3 GetCornerOfTheScreen(CornerType corner)
		{
			var camDepth = -MainCamera.transform.position.z;

			// MainCamera.ViewportToWorldPoint(new Vector3(1, 1, camDepth));
			var screenPosition = corner switch
			{
				CornerType.TopLeft => new Vector3(0, 1, camDepth),
				CornerType.TopRight => new Vector3(1, 1, camDepth),
				CornerType.BottomRight => new Vector3(1, 0, camDepth),
				CornerType.BottomLeft => new Vector3(0, 0, camDepth)
			};

			return MainCamera.ViewportToWorldPoint(screenPosition);
		}

		/// <summary>
		/// It tells you whether a position is inside a camera frustum.
		/// </summary>
		/// <param name="position">Given position point</param>
		/// <param name="camera">Camera</param>
		/// <returns>Is inside or not</returns>
		public static bool IsPositionInsideCamera(Vector3 position, Camera camera = null)
		{
			camera = camera ? camera : MainCamera;
			var viewport = camera.WorldToViewportPoint(position);
			bool inCameraFrustum = viewport.x.Is01() && viewport.y.Is01();
			bool inFrontOfCamera = viewport.z > 0;

			return inCameraFrustum && inFrontOfCamera;
		}

		/// <summary>
		/// Formats a big number to more readable manner
		/// <br/> 1000000 to 1M etc.
		/// </summary>
		/// <param name="number">Big number</param>
		/// <returns>String value of formatted number</returns>
		public static string FormatBigNumber(long number)
		{
			return number switch
			{
				> 999999999999999999 => number.ToString("0,,,,,,.##Q", CultureInfo.InvariantCulture),
				> 999999999999999 => number.ToString("0,,,,,.##q", CultureInfo.InvariantCulture),
				> 999999999999 => number.ToString("0,,,,.##T", CultureInfo.InvariantCulture),
				> 999999999 => number.ToString("0,,,.##B", CultureInfo.InvariantCulture),
				> 999999 => number.ToString("0,,.##M", CultureInfo.InvariantCulture),
				> 999 => number.ToString("0,.##K", CultureInfo.InvariantCulture),
				_ => number.ToString(CultureInfo.InvariantCulture)
			};
		}
	}
}