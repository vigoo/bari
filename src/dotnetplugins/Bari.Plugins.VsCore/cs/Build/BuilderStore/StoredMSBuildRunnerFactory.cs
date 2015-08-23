using Bari.Core.Build.BuilderStore;
using Bari.Core.Generic;
using Bari.Plugins.VsCore.Model;

namespace Bari.Plugins.VsCore.Build.BuilderStore
{
    public class StoredMSBuildRunnerFactory: IMSBuildRunnerFactory
    {
        private readonly IMSBuildRunnerFactory baseImpl;
        private readonly IBuilderStore store;

        public StoredMSBuildRunnerFactory(IMSBuildRunnerFactory baseImpl, IBuilderStore store)
        {
            this.baseImpl = baseImpl;
            this.store = store;
        }

        public MSBuildRunner CreateMSBuildRunner(SlnBuilder slnBuilder, TargetRelativePath slnPath, MSBuildVersion version)
        {
            return store.Add(baseImpl.CreateMSBuildRunner(slnBuilder, slnPath, version));
        }
    }
}