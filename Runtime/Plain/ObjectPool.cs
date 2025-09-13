namespace MK.Pool
{
    public abstract class ObjectPool<T> : PoolContainer<T>, IObjectPool<T> where T : class, IPoolable
    {
        protected abstract T Create();

        T IObjectPool<T>.OnSpawn()
        {
            T instance;
            if (this.InstanceRecycled.Count > 0)
            {
                instance = this.InstanceRecycled[0];
                this.InstanceRecycled.RemoveAt(0);
            }
            else
            {
                instance = this.Create();
            }

            this.InstanceSpawned.Add(instance);
            instance.OnSpawned();
            PoolStatistics.TrackSpawned(instance);

            return instance;
        }
    }
}