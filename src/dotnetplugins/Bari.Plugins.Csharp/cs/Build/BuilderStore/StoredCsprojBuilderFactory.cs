using Bari.Core.Build.BuilderStore;
using Bari.Core.Model;

namespace Bari.Plugins.Csharp.Build.BuilderStore
{
    public class StoredCsprojBuilderFactory: ICsprojBuilderFactory
    {
        private readonly ICsprojBuilderFactory baseImpl;
        private readonly IBuilderStore store;

        public StoredCsprojBuilderFactory(ICsprojBuilderFactory baseImpl, IBuilderStore store)
        {
            this.baseImpl = baseImpl;
            this.store = store;
        }

        public CsprojBuilder CreateCsprojBuilder(Project project)
        {
            return store.Add(baseImpl.CreateCsprojBuilder(project));
        }
    }
}