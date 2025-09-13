namespace MK.Pool
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    public static class PoolStatistics
    {
#if UNITY_EDITOR
        public static readonly Dictionary<Type, (HashSet<IRecyclable> cached, HashSet<IRecyclable> spawned)>     TypeToPool   = new();
        public static readonly Dictionary<GameObject, (HashSet<GameObject> cached, HashSet<GameObject> spawned)> PrefabToPool = new();

        public static event Action OnSpawned;
        public static event Action OnRecycled;

        static PoolStatistics()
        {
            EditorApplication.playModeStateChanged += OnStateChanged;

            return;

            void OnStateChanged(PlayModeStateChange state)
            {
                EditorApplication.playModeStateChanged -= OnStateChanged;
                TypeToPool.Clear();
                PrefabToPool.Clear();
            }
        }
#endif

#if UNITY_EDITOR
        private static (HashSet<IRecyclable> cached, HashSet<IRecyclable> spawned) GetPool(Type type)
        {
            if (!TypeToPool.TryGetValue(type, out var pool))
            {
                TypeToPool[type] = pool = (new HashSet<IRecyclable>(), new HashSet<IRecyclable>());
            }

            return pool;
        }
#endif

        internal static void TrackSpawned<T>(T item) where T : IRecyclable
        {
#if UNITY_EDITOR
            var (cached, spawned) = GetPool(typeof(T));
            spawned.Add(item);
            cached.Remove(item);
            OnSpawned?.Invoke();
#endif
        }

        internal static void TrackRecycled<T>(T item) where T : IRecyclable
        {
#if UNITY_EDITOR
            var (cached, spawned) = GetPool(typeof(T));
            spawned.Remove(item);
            cached.Add(item);
            OnRecycled?.Invoke();
#endif
        }

        internal static void CleanUp<T>()
        {
#if UNITY_EDITOR
            TypeToPool.Remove(typeof(T));
#endif
        }

#if UNITY_EDITOR
        private static (HashSet<GameObject> cached, HashSet<GameObject> spawned) GetGameObjectPool(GameObject prefab)
        {
            if (!PrefabToPool.TryGetValue(prefab, out var pool))
            {
                PrefabToPool[prefab] = pool = (new HashSet<GameObject>(), new HashSet<GameObject>());
            }

            return pool;
        }
#endif

        internal static void TrackSpawnedGameObject(GameObject prefab, GameObject instance)
        {
#if UNITY_EDITOR
            var (cached, spawned) = GetGameObjectPool(prefab);
            spawned.Add(instance);
            cached.Remove(instance);
            OnSpawned?.Invoke();
#endif
        }

        internal static void TrackRecycledGameObject(GameObject prefab, GameObject instance)
        {
#if UNITY_EDITOR
            var (cached, spawned) = GetGameObjectPool(prefab);
            spawned.Remove(instance);
            cached.Add(instance);
            OnRecycled?.Invoke();
#endif
        }

        internal static void CleanUpGameObjectPool(GameObject prefab)
        {
#if UNITY_EDITOR
            PrefabToPool.Remove(prefab);
#endif
        }
    }
}