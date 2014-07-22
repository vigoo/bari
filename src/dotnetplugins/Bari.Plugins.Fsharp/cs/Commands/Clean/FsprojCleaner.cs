using System;
using System.Linq;
using Bari.Core.Commands.Clean;
using Bari.Core.Model;

namespace Bari.Plugins.Fsharp.Commands.Clean
{
    /// <summary>
    /// .fsproj files must be generated outside the target directory, into the project source tree otherwise
    /// visual studio does not show the hierarchy only a flat set of file references. This class performs the
    /// additional cleaning step to remove these generated project files.
    /// </summary>
    public class FsprojCleaner : ICleanExtension
    {
        private readonly Suite suite;

        /// <summary>
        /// Constructs the cleaner
        /// </summary>
        /// <param name="suite">Current suite</param>
        public FsprojCleaner(Suite suite)
        {
            this.suite = suite;
        }

        /// <summary>
        /// Performs the additional cleaning step
        /// </summary>
        /// <param name="parameters"></param>
        public void Clean(ICleanParameters parameters)
        {
            foreach (var projectRoot in from module in suite.Modules
                                        from project in module.Projects.Concat(module.TestProjects)
                                        select project.RootDirectory)
            {
                var fsRoot = projectRoot.GetChildDirectory("fs");
                if (fsRoot != null)
                {
                    foreach (var fsproj in fsRoot.Files.Where(
                        name => name.EndsWith(".fsproj", StringComparison.InvariantCultureIgnoreCase)))
                    {
                        fsRoot.DeleteFile(fsproj);
                    }

                    foreach (var fsproj in fsRoot.Files.Where(
                                name => name.EndsWith(".fsproj.user", StringComparison.InvariantCultureIgnoreCase)))
                    {
                        fsRoot.DeleteFile(fsproj);
                    }

                    foreach (var fsversion in projectRoot.Files.Where(
                                name => name.Equals("version.fs", StringComparison.InvariantCultureIgnoreCase)))
                    {
                        projectRoot.DeleteFile(fsversion);
                    }
                }
            }
        }
    }
}