namespace MK.Pool
{
    using System.Collections.Generic;

    public interface IPoolContainer<T> where T : class, IRecyclable
    {
        IReadOnlyCollection<T> InstanceSpawned { get; }

        void Recycle(T instance);
        void CleanUp();
    }
}