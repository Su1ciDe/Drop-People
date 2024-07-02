using UnityEngine;

namespace Fiber.Utilities
{
    public class SingletonPersistent<T> : SingletonBase where T : Component
    {
        public static T Instance { get; private set; } = null;
        
        protected override void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
        }
    }
}