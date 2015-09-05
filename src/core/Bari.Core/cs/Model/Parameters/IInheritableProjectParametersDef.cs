namespace Bari.Core.Model.Parameters
{
    public interface IInheritableProjectParametersDef
    {
        IInheritableProjectParameters CreateDefault(Suite suite, IInheritableProjectParameters parent);
    }
}