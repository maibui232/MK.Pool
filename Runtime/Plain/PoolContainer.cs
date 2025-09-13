namespace MK.Pool
{
    using System;
    using System.Collections.Generic;

    public abstract class PoolContainer<T> : IPoolContainer<T> where T : class, IRecyclable
    {
        protected internal readonly List<T> InstanceSpawned  = new();
        protected internal readonly List<T> InstanceRecycled = new();

        IReadOnlyCollection<T> IPoolContainer<T>.InstanceSpawned => this.InstanceSpawned;

        void IPoolContainer<T>.Recycle(T poolable)
        {
            if (!this.InstanceSpawned.Remove(poolable))
            {
                throw new InvalidOperationException($"Cannot recycle object of type {typeof(T).Name}");
            }

            this.InstanceRecycled.Add(poolable);
            poolable.OnRecycled();
            PoolStatistics.TrackRecycled(poolable);
        }

        public void Recycle(T instance)
        {
            throw new NotImplementedException();
        }

        void IPoolContainer<T>.CleanUp()
        {
            this.InstanceSpawned.Clear();
            this.InstanceRecycled.Clear();
            PoolStatistics.CleanUp<T>();
        }
    }
}