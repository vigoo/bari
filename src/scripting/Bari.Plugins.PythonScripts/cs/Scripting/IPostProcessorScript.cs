using Bari.Core.Model;

namespace Bari.Plugins.PythonScripts.Scripting
{
    public interface IPostProcessorScript : IScript
    {
        PostProcessorId PostProcessorId { get; }
    }
}