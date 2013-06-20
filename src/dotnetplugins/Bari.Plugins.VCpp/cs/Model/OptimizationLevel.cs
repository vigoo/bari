namespace Bari.Plugins.VCpp.Model
{
    /// <summary>
    /// Optimization levels for the C++ compiler CL.exe
    /// 
    /// <para>http://msdn.microsoft.com/en-us/library/k1ack8f1.aspx</para>
    /// </summary>
    public enum OptimizationLevel
    {
        Disabled,   // Od
        MinSpace,   // O1
        MaxSpeed,   // O2
        Full        // Ox
    }
}