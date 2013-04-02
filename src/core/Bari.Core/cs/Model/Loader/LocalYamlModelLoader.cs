using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using Bari.Core.Exceptions;
using YamlDotNet.RepresentationModel;

namespace Bari.Core.Model.Loader
{
    /// <summary>
    /// Model loader implementation which reads a YAML file from the local file system
    /// </summary>
    public sealed class LocalYamlModelLoader: YamlModelLoaderBase, IModelLoader
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof (LocalYamlModelLoader));

        /// <summary>
        /// Creates the model loader
        /// </summary>
        /// <param name="suiteFactory">Factory interface to create new suite instances</param>
        /// <param name="parametersLoaders">Parameter loader implementations</param>
        public LocalYamlModelLoader(ISuiteFactory suiteFactory, IEnumerable<IYamlProjectParametersLoader> parametersLoaders)
            : base(suiteFactory, parametersLoaders)
        {
            Contract.Requires(suiteFactory != null);
        }

        /// <summary>
        /// Returns true if the loader can load suite model from the given source
        /// </summary>
        /// <param name="source">Source, can mean anything (file names, urls, markup, etc.)</param>
        /// <returns>Returns <c>true</c> if the source has been identified and can be loaded.</returns>
        public bool Supports(string source)
        {
            if (File.Exists(source))
            {
                using (var reader = File.OpenText(source))
                {
                    string firstLine = reader.ReadLine();
                    return firstLine != null && firstLine.StartsWith("---");
                }
            }

            return false;
        }

        /// <summary>
        /// Loads a suite model from the given source
        /// 
        /// <para>It is guaranteed that this method will only be called if <see cref="IModelLoader.Supports"/> return <c>true</c>
        /// for the same source.</para>
        /// </summary>
        /// <param name="source">Source, can mean anything (file names, urls, markup, etc.)</param>
        /// <returns>Returns the loaded suite model. Never returns <c>null</c>. On error it throws an exception.</returns>
        public Suite Load(string source)
        {
            log.DebugFormat("Loading YAML suite specification from local file {0}...", source);

            using (var reader = File.OpenText(source))
            {
                var yaml = new YamlStream();
                yaml.Load(reader);

                if (yaml.Documents.Count == 1 &&
                    yaml.Documents[0] != null &&
                    yaml.Documents[0].RootNode != null)
                    return LoadYaml(yaml.Documents[0]);
                else
                    throw new InvalidSpecificationException(
                        string.Format("The source file ({0}) contains multiple yaml documents!", source));
            }
        }
    }
}