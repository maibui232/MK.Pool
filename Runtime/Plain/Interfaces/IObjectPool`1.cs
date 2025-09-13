namespace MK.Pool
{
    public interface IObjectPool<TPoolable, in TParam> : IPoolContainer<TPoolable> where TPoolable : class, IPoolable<TParam>
    {
        TPoolable Spawn(TParam param);
    }
}