using System;
using System.Collections.Generic;
using Fiber.Managers;
using TriInspector;
using UnityEngine;

namespace Fiber.Utilities
{
	public class ParticlePooler : Singleton<ParticlePooler>
	{
		[Serializable]
		public class Pool
		{
			[Tooltip("Give a tag to the pool to call")]
			public string Tag;
			[Tooltip("Prefab of the Particle to be pooled")]
			public GameObject Prefab;
			[Tooltip("The size (count) of the pool")]
			public int Size;
			[Tooltip("Whether the Particle deactivates itself after finished playing")]
			public bool AutoDeactivate;
		}

		[TableList]
		[SerializeField] private List<Pool> pools = new List<Pool>();
		private readonly Dictionary<string, Queue<ParticleSystem>> poolDictionary = new Dictionary<string, Queue<ParticleSystem>>();

		private void Awake()
		{
			InitPool();
		}

		private void InitPool()
		{
			foreach (var pool in pools)
				AddToPool(pool.Tag, pool.Prefab, pool.Size, pool.AutoDeactivate);
		}

		private void OnEnable()
		{
			LevelManager.OnLevelUnload += OnLevelUnload;
		}

		private void OnDisable()
		{
			LevelManager.OnLevelUnload -= OnLevelUnload;
		}

		private void OnLevelUnload() => DisableAllPooledObjects();

		private void DisableAllPooledObjects()
		{
			foreach (var pool in poolDictionary.Values)
			{
				foreach (var go in pool)
				{
					go.Stop();
					go.transform.SetParent(transform);
					go.gameObject.SetActive(false);
				}
			}
		}

		/// <summary>
		/// Spawns the pooled particle to given position
		/// </summary>
		/// <param name="poolTag">Tag of the particle to be spawned</param>
		/// <param name="position">Set the world position of the particle</param>
		/// <returns>The particle found matching the tag specified</returns>
		public ParticleSystem Spawn(string poolTag, Vector3 position)
		{
			var particle = SpawnFromPool(poolTag);

			particle.transform.position = position;
			return particle;
		}

		/// <summary>
		/// Spawns the pooled particle to given position and rotation
		/// </summary>
		/// <param name="poolTag">Tag of the particle to be spawned</param>
		/// <param name="position">Set the world position of the particle</param>
		/// <param name="rotation">Set the rotation of the particle</param>
		/// <returns>The particle found matching the tag specified</returns>
		public ParticleSystem Spawn(string poolTag, Vector3 position, Quaternion rotation)
		{
			var particle = SpawnFromPool(poolTag);

			particle.transform.position = position;
			particle.transform.rotation = rotation;
			return particle;
		}

		/// <summary>
		/// Spawns the pooled particle and parents the particle to given Transform
		/// </summary>
		/// <param name="poolTag">Tag of the particle to be spawned</param>
		/// <param name="parent">Parent that will be assigned to the particle</param>
		/// <param name="keepWorldRotation">Whether you want the rotation of the particle is same with its parent</param>
		/// <returns>The particle found matching the tag specified</returns>
		public ParticleSystem Spawn(string poolTag, Transform parent, bool keepWorldRotation = false)
		{
			var particle = SpawnFromPool(poolTag);

			var pTransform = particle.transform;
			pTransform.SetParent(parent);
			pTransform.localPosition = Vector3.zero;
			if (!keepWorldRotation)
				pTransform.forward = parent.forward;
			return particle;
		}

		/// <summary>
		/// Spawns the pooled particle to given position and parents the particle to given Transform
		/// </summary>
		/// <param name="poolTag">Tag of the particle to be spawned</param>
		/// <param name="position">Set the world position of the particle</param>
		/// <param name="parent">Parent that will be assigned to the particle</param>
		/// <returns>The particle found matching the tag specified</returns>
		public ParticleSystem Spawn(string poolTag, Vector3 position, Transform parent)
		{
			var particle = SpawnFromPool(poolTag);

			var pTransform = particle.transform;
			pTransform.position = position;
			pTransform.forward = parent.forward;
			pTransform.SetParent(parent);
			return particle;
		}

		/// <summary>
		/// Spawns the pooled particle to given position and rotation and parents the particle to given Transform
		/// </summary>
		/// <param name="poolTag">Tag of the particle to be spawned</param>
		/// <param name="position">Set the world position of the particle</param>
		/// <param name="rotation">Set the rotation of the particle</param>
		/// <param name="parent">Parent that will be assigned to the particle</param>
		/// <returns>The particle found matching the tag specified</returns>
		public ParticleSystem Spawn(string poolTag, Vector3 position, Quaternion rotation, Transform parent)
		{
			var particle = SpawnFromPool(poolTag);

			var pTransform = particle.transform;
			pTransform.position = position;
			pTransform.rotation = rotation;
			pTransform.SetParent(parent);
			return particle;
		}

		private ParticleSystem SpawnFromPool(string poolTag)
		{
			if (!poolDictionary.ContainsKey(poolTag))
			{
				Debug.Log("\"" + poolTag + "\" tag doesn't exist!");
				return null;
			}

			var particle = poolDictionary[poolTag].Dequeue();
			particle.gameObject.SetActive(true);
			particle.Play();

			poolDictionary[poolTag].Enqueue(particle);

			return particle;
		}

		/// <summary>
		/// Creates a new pool with defined tag and object of the particle
		/// </summary>
		/// <param name="poolTag">Tag for spawning particles</param>
		/// <param name="prefab">Particle to be pooled</param>
		/// <param name="count">Count of the pool</param>
		/// <param name="deactivate">Whether the Particle deactivates itself after finished playing</param>
		public void AddToPool(string poolTag, GameObject prefab, int count, bool deactivate = true)
		{
			if (poolDictionary.ContainsKey(poolTag))
			{
				Debug.LogWarning(gameObject.name + ": \"" + poolTag + "\" Tag has already exists! Skipped.");
				return;
			}

			var queue = new Queue<ParticleSystem>();
			for (int i = 0; i < count; i++)
			{
				var particle = Instantiate(prefab, transform).GetComponent<ParticleSystem>();
				if (deactivate)
				{
					var main = particle.main;
					main.stopAction = ParticleSystemStopAction.Disable;
				}

				particle.gameObject.SetActive(false);
				queue.Enqueue(particle);
			}

			poolDictionary.Add(poolTag, queue);
		}
	}
}