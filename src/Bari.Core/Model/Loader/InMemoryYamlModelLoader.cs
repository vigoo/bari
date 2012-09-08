using System.Diagnostics.Contracts;
using System.IO;
using Bari.Core.Exceptions;
using Ninject.Syntax;
using YamlDotNet.RepresentationModel;

namespace Bari.Core.Model.Loader
{
    /// <summary>
    /// Model loader implementation which loads a YAML document directly from a string
    /// 
    /// <remarks>Used primarily for testing</remarks>
    /// </summary>
    public sealed class InMemoryYamlModelLoader : YamlModelLoaderBase, IModelLoader
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof (InMemoryYamlModelLoader));

        /// <summary>
        /// Creates the model loader
        /// </summary>
        /// <param name="root">Path to resolve instances</param>
        public InMemoryYamlModelLoader(IResolutionRoot root) : base(root)
        {
            Contract.Requires(root != null);
        }

        /// <summary>
        /// Returns true if the loader can load suite model from the given source
        /// </summary>
        /// <param name="source">Source, can mean anything (file names, urls, markup, etc.)</param>
        /// <returns>Returns <c>true</c> if the source has been identified and can be loaded.</returns>
        public bool Supports(string source)
        {
            return source.StartsWith("---");
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
            log.Debug("Loading in-memory YAML suite specification...");

            using (var reader = new StringReader(source))
            {
                var yaml = new YamlStream();
                yaml.Load(reader);

                if (yaml.Documents.Count == 1 &&
                    yaml.Documents[0] != null &&
                    yaml.Documents[0].RootNode != null)
                    return LoadYaml(yaml.Documents[0]);
                else
                    throw new InvalidSpecificationException(
                        string.Format("The yaml source contains multiple yaml documents!"));
            }
        }
    }
}