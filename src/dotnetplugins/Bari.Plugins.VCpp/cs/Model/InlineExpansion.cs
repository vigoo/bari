namespace Bari.Plugins.VCpp.Model
{
    /// <summary>
    /// Inline function expansion settings for cl.exe 
    /// 
    /// <para>http://msdn.microsoft.com/en-us/library/47238hez.aspx</para>
    /// </summary>
    public enum InlineExpansion
    {
        Default,
        Disabled,               // Ob0
        OnlyExplicitInline,    // Ob1
        AnySuitable             // Ob2
    }
}