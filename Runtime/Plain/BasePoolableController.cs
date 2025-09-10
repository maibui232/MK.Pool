namespace MK.Pool
{
    using System;
    using System.Collections.Generic;

    public abstract class BasePoolableController<T> : IPoolableController<T> where T : class, IPoolable
    {
        private readonly List<T> elementCached  = new();
        private readonly List<T> elementSpawned = new();

        protected abstract T Create();

#region IPoolableController

        public IReadOnlyCollection<T> GetCached => this.elementCached;

        public IReadOnlyCollection<T> GetSpawned => this.elementSpawned;

        public T Instantiate()
        {
            T element;
            if (this.elementCached.Count > 0)
            {
                element = this.elementCached[0];
                this.elementCached.RemoveAt(0);
            }
            else
            {
                element = this.Create();
            }

            this.elementSpawned.Add(element);
            element.OnSpawn();

            PoolStatistics.TrackSpawned(element);

            return element;
        }

        public void Recycle(T instance)
        {
            if (!this.elementSpawned.Remove(instance))
            {
                throw new ArgumentException($"Element {instance} is not spawned in pool with type {typeof(T).FullName}");
            }

            this.elementCached.Add(instance);
            instance.OnRecycle();
            PoolStatistics.TrackRecycled(instance);
        }

        public void CleanUp()
        {
            this.elementSpawned.Clear();
            this.elementCached.Clear();
            PoolStatistics.CleanUp<T>();
        }

#endregion
    }
}