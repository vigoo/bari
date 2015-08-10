using Bari.Core.Build.BuilderStore;
using Bari.Core.Model;

namespace Bari.Plugins.VCpp.Build.BuilderStore
{
    public class StoredVcxprojBuilderFactory: IVcxprojBuilderFactory
    {
        private readonly IVcxprojBuilderFactory baseImpl;
        private readonly IBuilderStore store;

        public StoredVcxprojBuilderFactory(IVcxprojBuilderFactory baseImpl, IBuilderStore store)
        {
            this.baseImpl = baseImpl;
            this.store = store;
        }

        public VcxprojBuilder CreateFsprojBuilder(Project project)
        {
            return store.Add(baseImpl.CreateFsprojBuilder(project));
        }
    }
}