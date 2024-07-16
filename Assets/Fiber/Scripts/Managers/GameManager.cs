using Fiber.Utilities;
using ScriptableObjects;
using UnityEngine;

namespace Fiber.Managers
{
	[DefaultExecutionOrder(-1)]
	public class GameManager : SingletonInit<GameManager>
	{
		[SerializeField] private PersonMaterialsSO personMaterialsSO;
		public PersonMaterialsSO PersonMaterialsSO => personMaterialsSO;
		
		protected override void Awake()
		{
			base.Awake();
			Application.targetFrameRate = 60;
			Debug.unityLogger.logEnabled = Debug.isDebugBuild;
		}
	}
}