namespace MK.Pool
{
    using System.Collections.Generic;

    public interface IPoolableController<T> where T : class, IPoolable
    {
        IEnumerable<T> GetCached  { get; }
        IEnumerable<T> GetSpawned { get; }

        T    Instantiate();
        void Recycle(T obj);
        void CleanUp();
    }
}