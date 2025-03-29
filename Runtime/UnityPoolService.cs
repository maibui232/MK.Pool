namespace MK.Pool
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using MK.AssetsManager;
    using MK.Log;
    using UnityEngine;
    using ILogger = MK.Log.ILogger;
    using Object = UnityEngine.Object;

    public class UnityPoolService : IUnityPoolService
    {
        private readonly IAssetsManager assetsManager;
        private readonly ILogger        logger;

        private readonly Dictionary<string, GameObjectPool> keyToPool = new();

        public UnityPoolService(IAssetsManager assetsManager, ILoggerManager loggerManager)
        {
            this.assetsManager = assetsManager;
            this.logger        = loggerManager.GetDefaultLogger();
        }

        UniTask IUnityPoolService.CreatePoolAsync<T>(int initialAmount)
        {
            return ((IUnityPoolService)this).CreatePoolAsync(typeof(T).Name, initialAmount);
        }

        void IUnityPoolService.CreatePool(GameObject original, int initialAmount)
        {
            var key  = original.name;
            var pool = new GameObject($"{key}-Pool").AddComponent<GameObjectPool>();
            Object.DontDestroyOnLoad(pool);
            pool.Create(original, initialAmount);

            this.keyToPool.Add(key, pool);
        }

        async UniTask IUnityPoolService.CreatePoolAsync(string key, int initialAmount)
        {
            if (this.keyToPool.ContainsKey(key))
            {
                this.logger.Fatal(new Exception($"Exist pool with key: {key}"));

                return;
            }

            ((IUnityPoolService)this).CreatePool(await this.assetsManager.LoadAsync<GameObject>(key), initialAmount);
        }

        T IUnityPoolService.Spawn<T>(T original)
        {
            return ((IUnityPoolService)this).Spawn(original.gameObject).GetComponent<T>();
        }

        GameObject IUnityPoolService.Spawn(GameObject original)
        {
            var key = original.name;
            if (!this.keyToPool.ContainsKey(key))
            {
                ((IUnityPoolService)this).CreatePool(original, 1);
            }

            return this.keyToPool[key].Spawn();
        }

        T IUnityPoolService.Spawn<T>(string addressableKey)
        {
            return ((IUnityPoolService)this).Spawn(addressableKey).GetComponent<T>();
        }

        GameObject IUnityPoolService.Spawn(string addressableKey)
        {
            return ((IUnityPoolService)this).SpawnAsync(addressableKey).GetAwaiter().GetResult();
        }

        async UniTask<T> IUnityPoolService.SpawnAsync<T>(string addressableKey)
        {
            var obj = await ((IUnityPoolService)this).SpawnAsync(addressableKey);

            return obj.GetComponent<T>();
        }

        async UniTask<GameObject> IUnityPoolService.SpawnAsync(string addressableKey)
        {
            var original = await this.assetsManager.LoadAsync<GameObject>(addressableKey);

            return ((IUnityPoolService)this).Spawn(original);
        }

        void IUnityPoolService.Recycle<T>(T element)
        {
            ((IUnityPoolService)this).Recycle(element.gameObject);
        }

        void IUnityPoolService.Recycle(GameObject element)
        {
            var pool = this.keyToPool.Values.FirstOrDefault(pool => pool.Contains(element));
            if (!pool)
            {
                this.logger.Fatal(new Exception($"Couldn't recycle GameObject: {element.name}"));

                return;
            }

            pool.Recycle(element);
        }

        void IUnityPoolService.Cleanup<T>()
        {
            this.InternalCleanup(typeof(T).Name);
        }

        void IUnityPoolService.Cleanup<T>(T original)
        {
            this.InternalCleanup(original.name);
        }

        void IUnityPoolService.Cleanup(GameObject original)
        {
            this.InternalCleanup(original.name);
        }

        void IUnityPoolService.Cleanup(string key)
        {
            this.InternalCleanup(key);
        }

        private void InternalCleanup(string key)
        {
            if (!this.keyToPool.TryGetValue(key, out var pool))
            {
                this.logger.Fatal($"Couldn't cleanup pool with key: {key}");

                return;
            }

            pool.Cleanup();
            this.keyToPool.Remove(key);
        }
    }
}