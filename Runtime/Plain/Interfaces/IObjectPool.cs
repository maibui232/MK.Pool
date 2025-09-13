namespace MK.Pool
{
    public interface IObjectPool<TPoolable> : IPoolContainer<TPoolable> where TPoolable : class, IPoolable
    {
        TPoolable OnSpawn();
    }
}