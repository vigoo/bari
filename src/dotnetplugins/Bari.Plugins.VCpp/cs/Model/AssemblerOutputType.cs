namespace Bari.Plugins.VCpp.Model
{
    /// <summary>
    /// Assembler output type for cl.exe
    /// 
    /// <para>http://msdn.microsoft.com/en-us/library/367y26c6.aspx</para>
    /// </summary>
    public enum AssemblerOutputType
    {
        NoListing,
        AssemblyCode,            // FA
        AssemblyAndMachineCode, // FAc
        AssemblyAndSourceCode,  // FAs    
        All                       // FAcs
    }
}