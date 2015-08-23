using System.Collections.Generic;
using Bari.Core.Build.MergingTag;
using Bari.Core.Generic;
using Bari.Core.Model;

namespace Bari.Core.Build
{
    public interface ICoreBuilderFactory
    {
        ContentBuilder CreateContentBuilder(Project project);
        MergingBuilder CreateMergingBuilder(IEnumerable<IBuilder> sourceBuilders, IMergingBuilderTag tag);
        CopyResultBuilder CreateCopyResultBuilder(IBuilder sourceBuilder, IFileSystemDirectory targetDirectory);
    }
}