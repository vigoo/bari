using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Bari.Core.Model;
using Bari.Core.UI;
using NuGet;
using NuSelfUpdate;

namespace Bari.Core.Commands
{
    public class SelfUpdateCommand: ICommand
    {
#if !MONO
		private const string NUGET_PACKAGE_ID = "bari";
#else
		private const string NUGET_PACKAGE_ID = "bari-mono";
#endif

        private readonly IUserOutput output;

        public SelfUpdateCommand(IUserOutput output)
        {
            this.output = output;
        }

        /// <summary>
        /// Gets the name of the command. This is the string which can be used on the command line interface
        /// to access the particular command.
        /// </summary>
        public string Name
        {
            get { return "selfupdate"; }
        }

        /// <summary>
        /// Gets a short, one-liner description of the command
        /// </summary>
        public string Description
        {
            get { return "updates bari to its latest version"; }
        }

        /// <summary>
        /// Gets a detailed, multiline description of the command and its possible parameters, usage examples
        /// </summary>
        public string Help
        {
            get
            {
                return
                    @"=Self update command=

Downloads the latest *bari* from NuGet.
Example: `bari selfupdate`
";
            }
        }

        /// <summary>
        /// If <c>true</c>, the target goal is important for this command and must be explicitly specified by the user 
        /// (if the available goal set is not the default)
        /// </summary>
        public bool NeedsExplicitTargetGoal
        {
            get { return false; }
        }

        /// <summary>
        /// Runs the command
        /// </summary>
        /// <param name="suite">The current suite model the command is applied to</param>
        /// <param name="parameters">Parameters given to the command (in unprocessed form)</param>
        /// <returns>Returns <c>true</c> if the command succeeded</returns>
        public bool Run(Suite suite, string[] parameters)
        {
            var fileSystem = new ExtendedPhysicalFileSystem(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
            var updater = new AppUpdaterBuilder(NUGET_PACKAGE_ID)
                .FileSystemAccessedThrough(fileSystem)
                .Build();

            output.Message("Checking for updates (current version: {0})...", updater.CurrentVersion);
            var check = updater.CheckForUpdate();            
            
            if (check.UpdateAvailable)
            {
                output.Message("..found update: {0}", check.UpdatePackage.Version);
                output.Message("..preparing update..");
                var preparedUpdate = PrepareUpdate(updater, fileSystem, check.UpdatePackage);

                output.Message("..applying update..");
                updater.ApplyPreparedUpdate(preparedUpdate);

                output.Message("Update completed.");
                output.Warning("You should rebuild your suites with the updated bari before using them!");
                return true; 
            }
            else
            {
                output.Message("No updates available.");
                return false;
            }
        }

        private IPreparedUpdate PrepareUpdate(AppUpdater updater, IExtendedFileSystem fileSystem, IPackage package)
        {
            if (package == null || package.Id != "bari")
                throw new ArgumentNullException("package");

            if (package.Version < updater.CurrentVersion)
                throw new BackwardUpdateException(updater.CurrentVersion, package.Version);

            var prepDirectory = Path.Combine(fileSystem.AppDirectory, ".updates", package.Version.ToString());
            var preparedFiles = new List<string>();

            foreach (var packageFile in package.GetFiles("tools"))
            {
                var targetPath = Path.Combine(prepDirectory, Get(packageFile.Path, relativeTo: "app"));
                fileSystem.AddFile(targetPath, packageFile.GetStream());

                preparedFiles.Add(targetPath);
            }

            return new PreparedUpdate(package.Version, preparedFiles);        
        }

        string Get(string path, string relativeTo)
        {
            var pathSegments = new List<string>();
            var relativeToParentDir = Path.GetDirectoryName(relativeTo);

            var ignoreCase = StringComparison.InvariantCultureIgnoreCase;
            while (!relativeToParentDir.Equals(Path.GetDirectoryName(path), ignoreCase))
            {
                pathSegments.Add(Path.GetFileName(path));
                path = Path.GetDirectoryName(path);
            }

            return Path.Combine(pathSegments.AsEnumerable().Reverse().ToArray());
        }
    }
}