using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Monads;
using Bari.Core.Exceptions;

namespace Bari.Plugins.FSRepository.Model
{
    public class UriBasedPatternResolutionContext : IPatternResolutionContext
    {
        private readonly string repositoryName;
        private readonly string dependencyName;
        private readonly string fileName;
        private readonly string extension;
        private readonly string version;

        public UriBasedPatternResolutionContext(Uri uri)
        {
            Contract.Requires(uri != null);

            string host = uri.Host;
            string path = uri.AbsolutePath.TrimStart('/');

            string[] pathParts = path.Split('/');

            if (pathParts.Length == 1)
            {
                repositoryName = null;
                dependencyName = host;
                fileName = Path.GetFileNameWithoutExtension(pathParts[0]);
                extension = Path.GetExtension(pathParts[0]).With(ex => ex.TrimStart('.'));
                version = null;
            }
            else if (pathParts.Length == 2)
            {
                repositoryName = null;
                dependencyName = host;
                fileName = Path.GetFileNameWithoutExtension(pathParts[1]);
                extension = Path.GetExtension(pathParts[1]).With(ex => ex.TrimStart('.'));
                version = pathParts[0];
            }
            else if (pathParts.Length == 3)
            {
                repositoryName = host;
                dependencyName = pathParts[0];
                fileName = Path.GetFileNameWithoutExtension(pathParts[2]);
                extension = Path.GetExtension(pathParts[2]).With(ex => ex.TrimStart('.')); 
                version = pathParts[1];
            }
            else
            {
                throw new InvalidReferenceException("Invalid FS repository dependency URI: " + uri);
            }
        }

        public string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name);
        }

        public string RepositoryName
        {
            get { return repositoryName; }
        }

        public string DependencyName
        {
            get { return dependencyName; }
        }

        public string FileName
        {
            get { return fileName; }
        }

        public string Extension
        {
            get { return extension; }
        }

        public string Version
        {
            get { return version; }
        }
    }
}