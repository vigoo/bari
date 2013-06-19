namespace Bari.Plugins.VCpp.Model
{
    /// <summary>
    /// Exception handling model for cl.exe compilations
    /// 
    /// <para>http://msdn.microsoft.com/en-us/library/1deeycx5.aspx</para>
    /// </summary>
    public enum ExceptionHandlingType
    {
        None,
        Async,          // EHa
        Sync,           // EHsc
        SyncCThrow      // EHs
    }
}