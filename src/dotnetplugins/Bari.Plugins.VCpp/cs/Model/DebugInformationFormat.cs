namespace Bari.Plugins.VCpp.Model
{
    /// <summary>
    /// Debug information format parameter for cl.exe
    /// 
    /// <para>http://msdn.microsoft.com/en-us/library/958x11bc.aspx</para>
    /// </summary>
    public enum DebugInformationFormat
    {
        None,
        OldStyle,               // Z7
        ProgramDatabase,        // Zi
        EditAndContinue         // ZI
    }
}