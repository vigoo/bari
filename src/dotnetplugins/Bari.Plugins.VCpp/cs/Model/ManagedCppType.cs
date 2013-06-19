namespace Bari.Plugins.VCpp.Model
{
    /// <summary>
    /// Managed C++ compilation types (/clr)
    /// 
    /// <para>http://msdn.microsoft.com/en-us/library/k8d11d4s.aspx</para>
    /// </summary>
    public enum ManagedCppType
    {
        NotManaged,
        Managed,        // clr
        Pure,           // clr:pure
        Safe,           // clr:safe
        OldSyntax       // clr:oldSyntax
    }
}