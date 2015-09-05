using System;
using System.Globalization;
using System.Linq;
using System.Xml;
using Bari.Core.Model.Parameters;

namespace Bari.Plugins.VCpp.Model
{
    public class VCppProjectParametersBase<TSelf, TPropertyDefs> : InheritableProjectParameters<TSelf, TPropertyDefs> 
        where TPropertyDefs : ProjectParametersPropertyDefs<TSelf>, new() 
        where TSelf : InheritableProjectParameters<TSelf, TPropertyDefs>
    {
        public VCppProjectParametersBase(TSelf parent = null) : base(parent)
        {
        }

        protected void WriteStringArray(XmlWriter writer, string name, string[] array)
        {
            if (array != null && array.Length > 0)
                writer.WriteElementString(name,
                    String.Join(";", array.Select(warn => warn.ToString(CultureInfo.InvariantCulture))));
        }

    }
}