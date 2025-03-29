namespace MK.Pool
{
    public interface IPool<T> where T : class
    {
        bool Contains(T element);
        T    Spawn();
        void Recycle(T element);
        void Cleanup();
    }
}