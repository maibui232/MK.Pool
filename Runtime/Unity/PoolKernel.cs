namespace MK.Pool
{
    using MK.Kernel;

    public static class PoolKernel
    {
        public static void PoolConfigure(this IBuilder builder)
        {
            builder.Add<UnityPoolService>().AsImplementedInterface();
        }
    }
}