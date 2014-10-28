using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Bari.Core.Generic;
using Bari.Core.Model;

namespace Bari.Plugins.Nuget.Packager
{
    public class NuSpecGenerator
    {
        private readonly Suite suite;
        private readonly NugetPackagerParameters parameters;
        private readonly ISet<TargetRelativePath> files;

        public NuSpecGenerator(NugetPackagerParameters parameters, ISet<TargetRelativePath> files, Suite suite)
        {
            this.parameters = parameters;
            this.files = files;
            this.suite = suite;
        }

        public string ToXml()
        {
            using (var stringWriter = new StringWriter())
            using (var writer = new XmlTextWriter(stringWriter))
            {
                writer.WriteStartElement("package", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd");

                WriteMetadata(writer);
                WriteFiles(writer);

                writer.WriteEndElement();
                writer.Flush();
                return stringWriter.ToString();
            }
        }

        private void WriteMetadata(XmlWriter writer)
        {
            writer.WriteStartElement("metadata");
            writer.WriteElementString("id", parameters.Id);
            if (suite.Version != null)
                writer.WriteElementString("version", suite.Version);
            if (parameters.Title != null)
                writer.WriteElementString("title", parameters.Title);
            if (parameters.Authors != null)
                writer.WriteElementString("authors", String.Join(", ", parameters.Authors));
            writer.WriteElementString("description", parameters.Description);
            if (suite.Copyright != null)
                writer.WriteElementString("copyright", suite.Copyright);
            if (parameters.ProjectUrl != null)
                writer.WriteElementString("projectUrl", parameters.ProjectUrl.ToString());
            if (parameters.LicenseUrl != null)
                writer.WriteElementString("licenseUrl", parameters.LicenseUrl.ToString());
            if (parameters.IconUrl != null)
                writer.WriteElementString("iconUrl", parameters.IconUrl.ToString());
            if (parameters.Tags != null)
                writer.WriteElementString("tags", String.Join(", ", parameters.Tags));
            writer.WriteEndElement();
        }

        private void WriteFiles(XmlWriter writer)
        {
            writer.WriteStartElement("files");

            foreach (var targetRelativePath in files)
            {
                writer.WriteStartElement("file");
                writer.WriteAttributeString("src", targetRelativePath);

                if (parameters.PackageAsTool)
                    writer.WriteAttributeString("target", Path.Combine("tools", Path.GetDirectoryName(targetRelativePath.RelativePath)));

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }
    }
}