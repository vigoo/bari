using Bari.Core.Model;

namespace Bari.Core.Build.BuilderStore
{
    public class StoredReferenceBuilderFactory: IReferenceBuilderFactory
    {
        private readonly IReferenceBuilderFactory baseImpl;
        private readonly IBuilderStore store;

        public StoredReferenceBuilderFactory(IReferenceBuilderFactory baseImpl, IBuilderStore store)
        {
            this.baseImpl = baseImpl;
            this.store = store;
        }

        public IReferenceBuilder CreateReferenceBuilder(Reference reference, Project project)
        {
            var builder = baseImpl.CreateReferenceBuilder(reference, project);
            return store.Add(builder);
        }
    }
}