using Bari.Core.Model;

namespace Bari.Core.Build
{
    /// <summary>
    /// Special <see cref="IBuilder"/> family which gets a project reference (<see cref="Reference"/>)
    /// as parameter and builds (or gets) the referenced projects.
    /// 
    /// <para>
    /// Reference builder implementations must be bound to <see cref="IReferenceBuilder"/> interface
    /// in the root kernel, with a unique name which is the same as the reference URI's scheme.
    /// </para>
    /// </summary>
    public interface IReferenceBuilder: IBuilder
    {
        /// <summary>
        /// Gets or sets the reference to be resolved
        /// </summary>
        Reference Reference { get; set; }
    }
}