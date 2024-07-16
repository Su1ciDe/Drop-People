using ScriptableObjects;
using UnityEngine;

namespace Fiber.LevelSystem
{
	public class Level : MonoBehaviour
	{
		public LevelDataSO LevelDataSO { get; private set; }

		public virtual void Load(LevelDataSO levelDataSO)
		{
			LevelDataSO = levelDataSO;
			gameObject.SetActive(true);
		}

		public virtual void Play()
		{
		}
	}
}