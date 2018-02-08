using System;
using System.IO;
using Bari.Core.UI;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Setup.Configuration;

namespace Bari.Plugins.VsCore.Tools.Versions
{
    public class MSBuild40x86 : MSBuild
    {
        public MSBuild40x86(IParameters parameters) :
            base(parameters, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), @"Microsoft.NET\Framework\v4.0.30319\"))
        {
        }
    }
    public class MSBuild40x64 : MSBuild
    {
        public MSBuild40x64(IParameters parameters) :
            base(parameters, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), @"Microsoft.NET\Framework64\v4.0.30319\"))
        {
        }
    }
    public class MSBuildVS2013 : MSBuild
    {
        public MSBuildVS2013(IParameters parameters) :
            base(parameters, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"MSBuild\12.0\Bin\"))
        {
        }
    }
    public class MSBuildVS2015 : MSBuild
    {
        public MSBuildVS2015(IParameters parameters) :
            base(parameters, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"MSBuild\14.0\Bin\"))
        {
        }
    }
    public class MSBuildVS2017 : MSBuild
    {
        public MSBuildVS2017(IParameters parameters) :
            base(parameters, VSInstance.GetMsBuildInstallPath("15"))
        {
        }
    }
    public class MSBuildInPath : MSBuild
    {
        public MSBuildInPath(IParameters parameters) :
            base(parameters, "")
        {
        }
    }

    internal class VSInstance
    {
        private static IEnumerable<ISetupInstance> EnumerateVisualStudioInstances()
        {
            var setupConfiguration = new SetupConfiguration() as ISetupConfiguration2;

            var instanceEnumerator = setupConfiguration.EnumAllInstances();
            var instances = new ISetupInstance[3];

            var instancesFetched = 0;
            instanceEnumerator.Next(instances.Length, instances, out instancesFetched);

            if (instancesFetched == 0)
            {
                throw new Exception("There were no instances of Visual Studio 15.0 or later found.");
            }

            do
            {
                for (var index = 0; index < instancesFetched; index++)
                {
                    yield return instances[index];
                }

                instanceEnumerator.Next(instances.Length, instances, out instancesFetched);
            }
            while (instancesFetched != 0);
        }

        private static ISetupInstance LocateVisualStudioInstance(string vsProductVersion, HashSet<string> requiredPackageIds)
        {
            var insts = EnumerateVisualStudioInstances().ToList();
            var instances = insts.Where((instance) => instance.GetInstallationVersion().StartsWith(vsProductVersion));

            var instanceFoundWithInvalidState = false;

            foreach (ISetupInstance2 instance in instances.OrderByDescending(i => i.GetInstallationVersion()))
            {
                var packages = instance.GetPackages()
                    .Where((package) => requiredPackageIds.Contains(package.GetId()));

                if (packages.Count() != requiredPackageIds.Count)
                {
                    continue;
                }

                const InstanceState minimumRequiredState = InstanceState.Local | InstanceState.Registered;

                var state = instance.GetState();

                if ((state & minimumRequiredState) == minimumRequiredState)
                {
                    return instance;
                }

                instanceFoundWithInvalidState = true;
            }

            throw new Exception(instanceFoundWithInvalidState ?
                                "An instance matching the specified requirements was found but it was in an invalid state." :
                                "There were no instances of Visual Studio 15.0 or later found that match the specified requirements.");
        }

        internal static string GetMsBuildInstallPath(string mainVersion)
        {
            var instance = LocateVisualStudioInstance(mainVersion, new HashSet<string>(new[] { "Microsoft.Component.MSBuild" })) as ISetupInstance2;
            if (instance != null)
            {
                var installationPath = instance.GetInstallationPath();
                return Path.Combine(installationPath, "MSBuild", mainVersion + ".0", "Bin");
            }

            throw new Exception("There were no instances of Visual Studio " + mainVersion + " found.");
        }
    }
}