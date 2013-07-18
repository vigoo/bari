using System;
using System.Globalization;
using System.Linq;
using System.Xml;
using Bari.Core.Model;

namespace Bari.Plugins.VCpp.Model
{
    public class VCppProjectParametersBase : IProjectParameters
    {
        protected void WriteStringArray(XmlWriter writer, string name, string[] array)
        {
            if (array != null && array.Length > 0)
                writer.WriteElementString(name,
                    String.Join(";", array.Select(warn => warn.ToString(CultureInfo.InvariantCulture))));
        }

    }
}