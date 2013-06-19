namespace Bari.Plugins.VCpp.Model
{
    /// <summary>
    /// Run-time error check types for cl.exe
    /// 
    /// <para>http://msdn.microsoft.com/en-us/library/8wtf2dfz.aspx</para>
    /// </summary>
    public enum RuntimeCheckType
    {
        Default,
        StackFrameRuntimeCheck,         // RTCs
        UninitializedLocalUsageCheck,   // RTCu
        EnableFastChecks                // RTC1
    }
}