namespace MK.Pool
{
    public interface IPoolable
    {
        void OnSpawn();
        void OnRecycle();
    }
}