namespace MK.Pool
{
    public interface IPoolable<in TParam> : IRecyclable
    {
        void OnSpawned(TParam param);
    }
}