using System;
using System.Collections.Generic;
using Fiber.Managers;
using TriInspector;
using UnityEngine;

namespace Fiber.Utilities
{
	public class ObjectPooler : Singleton<ObjectPooler>
	{
		[Serializable]
		public class Pool
		{
			[Tooltip("Give a tag to the pool to call")]
			public string Tag;
			[Tooltip("The prefab to be pooled")]
			public GameObject Prefab;
			[Tooltip("The size (count) of the pool")]
			public int InitialSize = 10;
			[Tooltip("The max size (count) pool can reach")]
			public int MaxSize = 10;
		}

		private class Pooled
		{
			public Queue<GameObject> PooledObjectsQueue;
			public List<GameObject> ActiveList = new List<GameObject>();
			public int CountAll;
			public int CountMax;
			public int CountActive => ActiveList.Count;
			public int CountInactive => PooledObjectsQueue.Count;
		}
		
		[TableList]
		[SerializeField] private List<Pool> pools = new List<Pool>();
		private Dictionary<string, Pooled> poolDictionary = new Dictionary<string, Pooled>();

		private void Awake()
		{
			Initialize();
		}

		public ObjectPooler Initialize()
		{
			foreach (var pool in pools)
				AddToPool(pool.Tag, pool.Prefab, pool.InitialSize, pool.MaxSize);

			return this;
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
				var count = pool.ActiveList.Count;
				for (var i = 0; i < count; i++)
				{
					var go = pool.ActiveList[0];
					go.transform.SetParent(transform);
					go.gameObject.SetActive(false);
					pool.PooledObjectsQueue.Enqueue(go);
					pool.ActiveList.RemoveAt(0);
				}
			}
		}

		/// <summary>
		/// Spawns the pooled object to given position
		/// </summary>
		/// <param name="poolTag">Tag of the object to be spawned</param>
		/// <param name="position">Set the world position of the object</param>
		/// <returns>The object found matching the tag specified</returns>
		public GameObject Spawn(string poolTag, Vector3 position)
		{
			var obj = SpawnFromPool(poolTag);

			obj.transform.position = position;
			return obj;
		}

		/// <summary>
		/// Spawns the pooled object to given position and rotation
		/// </summary>
		/// <param name="poolTag">Tag of the object to be spawned</param>
		/// <param name="position">Set the world position of the object</param>
		/// <param name="rotation">Set the rotation of the object</param>
		/// <returns>The object found matching the tag specified</returns>
		public GameObject Spawn(string poolTag, Vector3 position, Quaternion rotation)
		{
			var obj = SpawnFromPool(poolTag);

			obj.transform.position = position;
			obj.transform.rotation = rotation;
			return obj;
		}

		/// <summary>
		/// Spawns the pooled object and parents the object to given Transform
		/// </summary>
		/// <param name="poolTag">Tag of the object to be spawned</param>
		/// <param name="parent">Parent that will be assigned to the object</param>
		/// <returns>The object found matching the tag specified</returns>
		public GameObject Spawn(string poolTag, Transform parent)
		{
			var obj = SpawnFromPool(poolTag);

			obj.transform.SetParent(parent);
			obj.transform.localPosition = Vector3.zero;
			obj.transform.forward = parent.forward;
			return obj;
		}

		/// <summary>
		/// Spawns the pooled object to given position and parents the object to given Transform
		/// </summary>
		/// <param name="poolTag">Tag of the object to be spawned</param>
		/// <param name="position">Set the world position of the object</param>
		/// <param name="parent">Parent that will be assigned to the object</param>
		/// <returns>The object found matching the tag specified</returns>
		public GameObject Spawn(string poolTag, Vector3 position, Transform parent, bool worldPosition = true)
		{
			var obj = SpawnFromPool(poolTag);

			if (worldPosition)
				obj.transform.position = position;
			else
				obj.transform.localPosition = position;

			obj.transform.forward = parent.forward;
			obj.transform.SetParent(parent);
			return obj;
		}

		/// <summary>
		/// Spawns the pooled object to given position and rotation and parents the object to given Transform
		/// </summary>
		/// <param name="poolTag">Tag of the object to be spawned</param>
		/// <param name="position">Set the world position of the object</param>
		/// <param name="rotation">Set the rotation of the object</param>
		/// <param name="parent">Parent that will be assigned to the object</param>
		/// <returns>The object found matching the tag specified</returns>
		public GameObject Spawn(string poolTag, Vector3 position, Quaternion rotation, Transform parent, bool worldPosition = true)
		{
			var obj = SpawnFromPool(poolTag);

			if (worldPosition)
				obj.transform.position = position;
			else
				obj.transform.localPosition = position;
			obj.transform.rotation = rotation;
			obj.transform.SetParent(parent);
			return obj;
		}

		private GameObject SpawnFromPool(string poolTag)
		{
			if (!poolDictionary.TryGetValue(poolTag, out var pooled)) return null;
			if (!pooled.CountInactive.Equals(0))
			{
				var obj = poolDictionary[poolTag].PooledObjectsQueue.Dequeue();
				obj.transform.localScale = Vector3.one;
				obj.SetActive(true);
				pooled.ActiveList.Add(obj);
				// If pool cannot grow more, 
				if (pooled.CountAll.Equals(pooled.CountMax))
				{
					poolDictionary[poolTag].PooledObjectsQueue.Enqueue(obj);
				}

				return obj;
			}
			else if (!pooled.CountAll.Equals(pooled.CountMax))
			{
				var obj = Instantiate(pooled.ActiveList[0], transform);
				obj.transform.localScale = Vector3.one;
				pooled.ActiveList.Add(obj);
				pooled.CountAll++;
				obj.SetActive(true);

				return obj;
			}
			else
			{
				for (int i = 0; i < pooled.CountActive; i++)
					pooled.PooledObjectsQueue.Enqueue(pooled.ActiveList[i]);
				var obj = pooled.ActiveList[0];
				return obj;
			}
		}

		/// <summary>
		/// Disable the GameObject and adds it back to the pool
		/// </summary>
		/// <param name="pooledGameObject">GameObject to be released</param>
		/// <param name="poolTag">Tag of the object to be released</param>
		public void Release(GameObject pooledGameObject, string poolTag)
		{
			if (!poolDictionary.TryGetValue(poolTag, out var pooled)) return;
			if (!pooled.ActiveList.Contains(pooledGameObject)) return;
			pooledGameObject.SetActive(false);
			pooledGameObject.transform.SetParent(transform);
			if (pooled.PooledObjectsQueue.Contains(pooledGameObject)) return;

			pooled.PooledObjectsQueue.Enqueue(pooledGameObject);
			pooled.ActiveList.Remove(pooledGameObject);
		}

		public IEnumerable<GameObject> Peek(string poolTag)
		{
			return poolDictionary[poolTag].PooledObjectsQueue;
		}

		/// <summary>
		/// Creates a new pool with defined tag and object
		/// </summary>
		/// <param name="poolTag">Tag for spawning objects</param>
		/// <param name="prefab">Object to be pooled</param>
		/// <param name="count">Initial count of the pool</param>
		/// <param name="maxCount">Max count of the pool</param>
		public void AddToPool(string poolTag, GameObject prefab, int count, int maxCount)
		{
			if (poolDictionary.ContainsKey(poolTag))
			{
				Debug.LogWarning(name + ": " + gameObject.name + ": \"" + poolTag + "\" Tag has already exists! Skipped.");
				return;
			}

			if (count > maxCount)
			{
				Debug.LogWarning(name + ": Max Count can't be smaller than Initial Count");
				return;
			}

			var queue = new Queue<GameObject>();
			for (int i = 0; i < count; i++)
			{
				var obj = Instantiate(prefab, transform);
				obj.SetActive(false);
				queue.Enqueue(obj);
			}

			var pooled = new Pooled { PooledObjectsQueue = queue, CountAll = count, CountMax = maxCount };
			poolDictionary.Add(poolTag, pooled);
		}
	}
}