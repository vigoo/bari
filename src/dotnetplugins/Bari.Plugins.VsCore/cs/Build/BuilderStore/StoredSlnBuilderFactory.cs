using System.Collections.Generic;
using Bari.Core.Build.BuilderStore;
using Bari.Core.Model;
using Bari.Plugins.VsCore.Model;

namespace Bari.Plugins.VsCore.Build.BuilderStore
{
    public class StoredSlnBuilderFactory: ISlnBuilderFactory
    {
        private readonly ISlnBuilderFactory baseImpl;
        private readonly IBuilderStore store;

        public StoredSlnBuilderFactory(ISlnBuilderFactory baseImpl, IBuilderStore store)
        {
            this.baseImpl = baseImpl;
            this.store = store;
        }

        public SlnBuilder CreateSlnBuilder(IEnumerable<Project> projects, MSBuildVersion msBuildVersion)
        {
            return store.Add(baseImpl.CreateSlnBuilder(projects, msBuildVersion));
        }
    }
}