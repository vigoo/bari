using System;
using System.Linq;
using Bari.Core.Commands.Clean;
using Bari.Core.Model;
using Bari.Plugins.VCpp.Model;

namespace Bari.Plugins.VCpp.Commands.Clean
{
    /// <summary>
    /// .vcxproj files must be generated outside the target directory, into the project source tree otherwise
    /// visual studio does not show the hierarchy only a flat set of file references. This class performs the
    /// additional cleaning step to remove these generated project files.
    /// </summary>
    public class VcxprojCleaner : ICleanExtension
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof (VcxprojCleaner));

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
            foreach (var project in from module in suite.Modules
                                        from project in module.Projects.Concat(module.TestProjects)
                                        select project)
            {
                var projectRoot = project.RootDirectory;
                var fsRoot = projectRoot.GetChildDirectory("cpp");
                if (fsRoot != null)
                {
                    foreach (var fsproj in fsRoot.Files.Where(
                        name => name.EndsWith(".vcxproj", StringComparison.InvariantCultureIgnoreCase)))
                    {
                        log.Debug("Deleting vxproj file");
                        fsRoot.DeleteFile(fsproj);
                    }

                    foreach (var fsproj in fsRoot.Files.Where(
                                name => name.EndsWith(".vcxproj.user", StringComparison.InvariantCultureIgnoreCase)))
                    {
                        log.Debug("Deleting vcxproj.user file");
                        fsRoot.DeleteFile(fsproj);
                    }

                    var filteredFiles = project.GetSourceSet("cpp").Files.Except(
                        project.GetSourceSet("cpp").FilterCppSourceSet(fsRoot, suite.SuiteRoot).Files);

                    foreach (var filteredFile in filteredFiles)
                    {
                        log.DebugFormat("Deleting generated file {0}", filteredFile);
                        suite.SuiteRoot.DeleteFile(filteredFile);
                    }
                }
            }
        }
    }
}