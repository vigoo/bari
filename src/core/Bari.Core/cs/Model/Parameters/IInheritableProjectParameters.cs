namespace Bari.Core.Model.Parameters
{
    public interface IInheritableProjectParameters: IProjectParameters
    {
        IInheritableProjectParametersDef Definition { get; }

        IInheritableProjectParameters WithParent(Suite suite, IInheritableProjectParameters newParent);
    }
}