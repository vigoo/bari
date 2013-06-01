namespace Bari.Core.Build.Cache
{
    public interface ICachedBuilderFactory
    {
        CachedBuilder CreateCachedBuilder(IBuilder wrappedBuilder);
    }
}