namespace MK.Pool
{
    using System.Collections.Generic;

    public interface IPoolableController<T> where T : class, IPoolable
    {
        IReadOnlyCollection<T> GetCached  { get; }
        IReadOnlyCollection<T> GetSpawned { get; }

        T    Instantiate();
        void Recycle(T obj);
        void CleanUp();
    }
}