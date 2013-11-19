namespace Bari.Plugins.VCpp.Model
{
    public class VCppProjectATLParameters: VCppProjectParametersBase
    {
        public UseOfATL UseOfATL { get; set; }

        public VCppProjectATLParameters()
        {
            UseOfATL = UseOfATL.None;
        }
    }
}