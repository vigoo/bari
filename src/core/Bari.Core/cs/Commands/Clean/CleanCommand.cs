using System;
using System.Collections.Generic;
using System.IO;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Core.UI;

namespace Bari.Core.Commands.Clean
{
    /// <summary>
    /// Implements the 'clean' command, which removes all the generated and cached files
    /// from the suite file system.
    /// </summary>
    public class CleanCommand : ICommand
    {
        private readonly IFileSystemDirectory targetRoot;
        private readonly IEnumerable<ICleanExtension> extensions;
        private readonly IUserOutput output;
        private readonly ISoftCleanPredicates predicates;

        /// <summary>
        /// Constructs the command
        /// </summary>
        /// <param name="targetRoot">Target root directory</param>
        /// <param name="extensions">Additional cleaning steps to be performed </param>
        /// <param name="output">User interface output interface</param>
        /// <param name="predicates">Soft clean predicate registry</param>
        public CleanCommand([TargetRoot] IFileSystemDirectory targetRoot, IEnumerable<ICleanExtension> extensions, IUserOutput output, ISoftCleanPredicates predicates)
        {
            this.targetRoot = targetRoot;
            this.extensions = extensions;
            this.output = output;
            this.predicates = predicates;
        }

        /// <summary>
        /// Gets the name of the command. This is the string which can be used on the command line interface
        /// to access the particular command.
        /// </summary>
        public string Name
        {
            get { return "clean"; }
        }

        /// <summary>
        /// Gets a short, one-liner description of the command
        /// </summary>
        public string Description
        {
            get { return "cleans that target dir and the bari cache"; }
        }

        /// <summary>
        /// Gets a detailed, multiline description of the command and its possible parameters, usage examples
        /// </summary>
        public string Help
        {
            get
            {
                return
@"=Clean command=

When used without parameter, it deletes the `target` and `cache` directories.
Example: `bari clean`

When used with the `--keep-references` option, it keeps the 3rd party references in the cache.
Example: `bari clean --keep-references`

When used with the `--soft-clean` option, it keeps some files not directly relate to the build,
such as `.suo` files.
Example: `bari clean --soft-clean`

The two options can be used together!
";

            }
        }

        /// <summary>
        /// If <c>true</c>, the target goal is important for this command and must be explicitly specified by the user 
        /// (if the available goal set is not the default)
        /// </summary>
        public bool NeedsExplicitTargetGoal
        {
            get { return true; }
        }

        /// <summary>
        /// Runs the command
        /// </summary>
        /// <param name="suite">The current suite model the command is applied to</param>
        /// <param name="parameters">Parameters given to the command (in unprocessed form)</param>
        public bool Run(Suite suite, string[] parameters)
        {
            var cleanParams = new CleanParameters(parameters);

            try
            {
                if (cleanParams.SoftClean)
                {
                    targetRoot.Delete(predicates.ShouldDelete);
                }
                else
                {
                    targetRoot.Delete();
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                CleanWarning(ex);                
            }
            catch (IOException ex)
            {
                CleanWarning(ex);
            }

            foreach (var cleanExtension in extensions)
            {
                cleanExtension.Clean(cleanParams);
            }

            return true;
        }

        private void CleanWarning(Exception ex)
        {
            output.Warning(String.Format("Failed to clean target root: {0}", ex.Message),
                new[]
                {
                    "A command prompt may have its current directory set there",
                    "Maybe the process is running"
                });
        }
    }
}