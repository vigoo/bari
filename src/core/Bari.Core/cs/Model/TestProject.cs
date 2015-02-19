using System.IO;
using Bari.Core.Generic;

namespace Bari.Core.Model
{
    /// <summary>
    /// TestProject is a special <see cref="Project"/>, containing unit tests and not used in regular builds
    /// </summary>
    public class TestProject: Project
    {
        /// <summary>
        /// Gets or sets the root directory of the project's sources
        /// </summary>
        public override IFileSystemDirectory RootDirectory
        {
            get { return Module.RootDirectory.GetChildDirectory("tests").GetChildDirectory(Name); }
        }

        public override string RelativeTargetPath
        {
            get { return Module.Name + ".tests"; }
        }

        /// <summary>
        /// Gets or sets the root directory of the project's sources
        /// </summary>
        public override string RelativeRootDirectory
        {
            get { return Path.Combine("src", Module.Name, "tests", Name); }
        }

        /// <summary>
        /// Constructs the test project
        /// </summary>
        /// <param name="name">Name of the project</param>
        /// <param name="module">Module where he project belongs to</param>
        public TestProject(string name, Module module) : base(name, module)
        {
        }
    }
}