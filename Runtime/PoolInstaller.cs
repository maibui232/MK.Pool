namespace MK.Pool
{
    using MK.DependencyInjection;

    public static class PoolInstaller
    {
        public static void InstallPool(this IBuilder builder)
        {
            builder.Register<UnityPoolService>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}