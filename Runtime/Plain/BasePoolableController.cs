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

        IEnumerable<T> IPoolableController<T>.GetCached => this.elementCached;

        IEnumerable<T> IPoolableController<T>.GetSpawned => this.elementSpawned;

        T IPoolableController<T>.Instantiate()
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

            return element;
        }

        void IPoolableController<T>.Recycle(T instance)
        {
            if (!this.elementSpawned.Remove(instance))
            {
                throw new ArgumentException($"Element {instance} is not spawned in pool with type {typeof(T).FullName}");
            }

            this.elementCached.Add(instance);
            instance.OnRecycle();
        }

        void IPoolableController<T>.CleanUp()
        {
            this.elementSpawned.Clear();
            this.elementCached.Clear();
        }

#endregion
    }
}