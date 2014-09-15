using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Bari.Core.Commands.Helper;
using Bari.Core.Model;

namespace Bari.Plugins.AddonSupport.Model
{
    public class AddonSupportSolutionItemData
    {
        private readonly ICommandTargetParser targetParser;
        private readonly IHasBuildTarget buildTargetSource;
        private readonly Goal activeGoal;
        private string targetName;

        public AddonSupportSolutionItemData(ICommandTargetParser targetParser, IHasBuildTarget buildTargetSource, Goal activeGoal)
        {
            this.targetParser = targetParser;
            this.buildTargetSource = buildTargetSource;
            this.activeGoal = activeGoal;
        }

        public string BariPath
        {
            get
            {
                return new Uri(Assembly.GetEntryAssembly().CodeBase).LocalPath;
            }
        }

        public string Goal
        {
            get
            {
                return activeGoal.Name;
            }
        }

        public string Target
        {
            get
            {
                return targetName ?? (targetName = FindOutCurrentTarget());
            }
        }

        public string StartupPath
        {
            get
            {
                return FindOutResultExecutablePath(Target);
            }
        }


        private string FindOutResultExecutablePath(string targetStr)
        {
            Project exeProject = null;

            var target = targetParser.ParseTarget(targetStr);
            var productTarget = target as ProductTarget;
            if (productTarget != null)
            {
                if (productTarget.Product.HasParameters("startup"))
                {
                    var startupParams = productTarget.Product.GetParameters<StartupModuleParameters>("startup");
                    exeProject = startupParams.Project;
                }
            }

            if (exeProject == null)
            {
                exeProject =
                    target.Projects.FirstOrDefault(
                        prj => prj.Type == ProjectType.Executable || prj.Type == ProjectType.WindowsExecutable);
            }

            if (exeProject != null)
                return Path.Combine(exeProject.RelativeTargetPath, exeProject.Name + ".exe");
            else
                return String.Empty;
        }

        private string FindOutCurrentTarget()
        {
            if (buildTargetSource != null)
                return buildTargetSource.BuildTarget;
            else
                return String.Empty;
        }
    }
}