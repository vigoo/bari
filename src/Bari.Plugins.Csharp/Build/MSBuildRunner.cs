using System.Collections.Generic;
using Bari.Core.Build;
using Bari.Core.Build.Dependencies;
using Bari.Core.Generic;
using Bari.Plugins.Csharp.Tools;

namespace Bari.Plugins.Csharp.Build
{
    /// <summary>
    /// Builder for running MSBuild on a Visual Studio solution file.
    /// </summary>
    public class MSBuildRunner: IBuilder
    {
        private readonly IBuilder slnBuilder;
        private readonly TargetRelativePath slnPath;
        private readonly IFileSystemDirectory targetRoot;
        private readonly IMSBuild msbuild;
        private readonly ISet<TargetRelativePath> outputs;

        /// <summary>
        /// Constructs the builder
        /// </summary>
        /// <param name="slnBuilder">Sub task building the solution file, used as a dependency</param>
        /// <param name="slnPath">Path of the generated solution file</param>
        /// <param name="outputs">Expected outputs of the MSBuild process</param>
        /// <param name="targetRoot">Target directory</param>
        /// <param name="msbuild">The MSBuild implementation to use</param>
        public MSBuildRunner(IBuilder slnBuilder, TargetRelativePath slnPath, IEnumerable<TargetRelativePath> outputs,
                             [TargetRoot] IFileSystemDirectory targetRoot, IMSBuild msbuild)
        {
            this.slnBuilder = slnBuilder;
            this.slnPath = slnPath;
            this.outputs = new HashSet<TargetRelativePath>(outputs);
            this.targetRoot = targetRoot;
            this.msbuild = msbuild;
        }

        /// <summary>
        /// Dependencies required for running this builder
        /// </summary>
        public IDependencies Dependencies
        {
            get
            {
                return new SubtaskDependency(slnBuilder);
            }
        }

        /// <summary>
        /// Runs this builder
        /// </summary>
        /// <returns>Returns a set of generated files, in target relative paths</returns>
        public ISet<TargetRelativePath> Run()
        {
            msbuild.Run(targetRoot, slnPath);
            return outputs;
        }
    }
}