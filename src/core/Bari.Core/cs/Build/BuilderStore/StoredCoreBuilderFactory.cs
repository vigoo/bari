using System.Collections.Generic;
using Bari.Core.Generic;
using Bari.Core.Model;

namespace Bari.Core.Build.BuilderStore
{
    public class StoredCoreBuilderFactory: ICoreBuilderFactory
    {
        private readonly IBuilderStore store;
        private readonly ICoreBuilderFactory baseImpl;

        public StoredCoreBuilderFactory(ICoreBuilderFactory baseImpl, IBuilderStore store)
        {
            this.store = store;
            this.baseImpl = baseImpl;
        }

        public ContentBuilder CreateContentBuilder(Project project)
        {
            var builder = baseImpl.CreateContentBuilder(project);
            return store.Add(builder);
        }

        public MergingBuilder CreateMergingBuilder(IEnumerable<IBuilder> sourceBuilders)
        {
            var builder = baseImpl.CreateMergingBuilder(sourceBuilders);
            return store.Add(builder);
        }

        public CopyResultBuilder CreateCopyResultBuilder(IBuilder sourceBuilder, IFileSystemDirectory targetDirectory)
        {
            var builder = baseImpl.CreateCopyResultBuilder(sourceBuilder, targetDirectory);
            return store.Add(builder);
        }
    }
}