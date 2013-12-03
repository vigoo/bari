namespace Bari.Plugins.VCpp.Model
{
    public class VCppProjectCLIParameters : VCppProjectParametersBase
    {
        public CppCliMode Mode { get; set; }

        public VCppProjectCLIParameters()
        {
            Mode = CppCliMode.Disabled;
        }
    }
}