using Bari.Core.Model;
using Bari.Plugins.VCpp.Model;

namespace Bari.Plugins.VCpp.VisualStudio
{
    public static class ProjectExtensions
    {
        /// <summary>
        /// Gets the cli mode.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns></returns>
        public static CppCliMode GetCLIMode(this Project project)
        {
            VCppProjectCLIParameters cliParameters =
                project.GetInheritableParameters<VCppProjectCLIParameters, VCppProjectCLIParametersDef>("cli");

            return cliParameters.IsModeSpecified ? cliParameters.Mode : CppCliMode.Disabled;
        }
    }
}
