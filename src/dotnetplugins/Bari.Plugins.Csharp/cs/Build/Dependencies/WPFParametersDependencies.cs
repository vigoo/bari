using Bari.Core.Build;
using Bari.Core.Build.Dependencies;
using Bari.Core.Model;
using Bari.Plugins.Csharp.Model;

namespace Bari.Plugins.Csharp.Build.Dependencies
{
    public class WPFParametersDependencies : IDependencies
    {
        private readonly WPFParameters wpfParameters;

        public WPFParametersDependencies(Project project)
        {
            wpfParameters = project.GetParameters<WPFParameters>("wpf");
        }

        public IDependencyFingerprint CreateFingerprint()
        {
            return new ObjectPropertiesFingerprint(
                wpfParameters,
                new[] { "ApplicationDefinition" });
        }
    }
}