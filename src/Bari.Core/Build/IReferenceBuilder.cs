using Bari.Core.Model;

namespace Bari.Core.Build
{
    public interface IReferenceBuilder: IBuilder
    {
        Reference Reference { get; set; }
    }
}