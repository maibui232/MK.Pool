namespace MK.Pool
{
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    public interface IUnityPoolService
    {
        UniTask CreatePoolAsync<T>(int initialAmount) where T : Component;

        void CreatePool(GameObject original, int initialAmount);

        UniTask CreatePoolAsync(string key, int initialAmount);

        T          Spawn<T>(T       original) where T : Component;
        GameObject Spawn(GameObject spawn);

        T          Spawn<T>(string addressableKey) where T : Component;
        GameObject Spawn(string    addressableKey);

        UniTask<T>          SpawnAsync<T>(string addressableKey) where T : Component;
        UniTask<GameObject> SpawnAsync(string    addressableKey);

        void Recycle<T>(T       element) where T : Component;
        void Recycle(GameObject element);

        void Cleanup<T>() where T : Component;
        void Cleanup<T>(T       original) where T : Component;
        void Cleanup(GameObject original);

        void Cleanup(string key);
    }
}