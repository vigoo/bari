using Bari.Core.UI;

namespace Bari.Core.Build.BuilderStore
{
    public interface IBuilderStore
    {
        T Add<T>(T builder)
            where T : IBuilder;

        void DumpStats(IUserOutput output);
    }
}