using Bari.Core.Build.BuilderStore;
using Bari.Core.Model;

namespace Bari.Plugins.Fsharp.Build.BuilderStore
{
    public class StoredFsprojBuilderFactory: IFsprojBuilderFactory
    {
        private readonly IFsprojBuilderFactory baseImpl;
        private readonly IBuilderStore store;

        public StoredFsprojBuilderFactory(IFsprojBuilderFactory baseImpl, IBuilderStore store)
        {
            this.baseImpl = baseImpl;
            this.store = store;
        }

        public FsprojBuilder CreateFsprojBuilder(Project project)
        {
            return store.Add(baseImpl.CreateFsprojBuilder(project));
        }
    }
}