namespace MK.Pool
{
    public abstract class ObjectPool<TPoolable, TParam> : PoolContainer<TPoolable>, IObjectPool<TPoolable, TParam> where TPoolable : class, IPoolable<TParam>
    {
        protected abstract TPoolable Create(TParam param);

        TPoolable IObjectPool<TPoolable, TParam>.Spawn(TParam param)
        {
            TPoolable instance;
            if (this.InstanceRecycled.Count > 0)
            {
                instance = this.InstanceRecycled[0];
                this.InstanceRecycled.RemoveAt(0);
            }
            else
            {
                instance = this.Create(param);
            }

            this.InstanceSpawned.Add(instance);
            instance.OnSpawned(param);
            PoolStatistics.TrackSpawned(instance);

            return instance;
        }
    }
}