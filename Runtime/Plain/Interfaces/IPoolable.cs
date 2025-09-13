namespace MK.Pool
{
    public interface IPoolable : IRecyclable
    {
        void OnSpawned();
    }
}