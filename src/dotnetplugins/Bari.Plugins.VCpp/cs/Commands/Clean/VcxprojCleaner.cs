using System;
using System.Linq;
using Bari.Core.Commands.Clean;
using Bari.Core.Model;

namespace Bari.Plugins.VCpp.Commands.Clean
{
    /// <summary>
    /// .vcxproj files must be generated outside the target directory, into the project source tree otherwise
    /// visual studio does not show the hierarchy only a flat set of file references. This class performs the
    /// additional cleaning step to remove these generated project files.
    /// </summary>
    public class VcxprojCleaner : ICleanExtension
    {
        private readonly Suite suite;

        /// <summary>
        /// Constructs the cleaner
        /// </summary>
        /// <param name="suite">Current suite</param>
        public VcxprojCleaner(Suite suite)
        {
            this.suite = suite;
        }

        /// <summary>
        /// Performs the additional cleaning step
        /// </summary>
        public void Clean()
        {
            foreach (var projectRoot in from module in suite.Modules
                                        from project in module.Projects.Concat(module.TestProjects)
                                        select project.RootDirectory)
            {
                var fsRoot = projectRoot.GetChildDirectory("cpp");
                if (fsRoot != null)
                {
                    foreach (var fsproj in fsRoot.Files.Where(
                        name => name.EndsWith(".vcxproj", StringComparison.InvariantCultureIgnoreCase)))
                    {
                        fsRoot.DeleteFile(fsproj);
                    }

                    foreach (var fsproj in fsRoot.Files.Where(
                                name => name.EndsWith(".vcxproj.user", StringComparison.InvariantCultureIgnoreCase)))
                    {
                        fsRoot.DeleteFile(fsproj);
                    }
                }
            }
        }
    }
}