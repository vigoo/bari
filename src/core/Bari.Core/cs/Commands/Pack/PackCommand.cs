using System;
using System.Collections.Generic;
using System.IO;
using Bari.Core.Exceptions;
using Bari.Core.Generic;
using Bari.Core.Model;

namespace Bari.Core.Commands.Pack
{
    /// <summary>
    /// Implements the `pack` command which creates a single package from the build output of a <see cref="Product"/>
    /// </summary>
    public class PackCommand: ICommand
    {
        private readonly IProductPackagerFactory productPackagerFactory;
        private readonly IFileSystemDirectory targetRoot;

        /// <summary>
        /// Gets the name of the command. This is the string which can be used on the command line interface
        /// to access the particular command.
        /// </summary>
        public string Name
        {
            get { return "pack"; }
        }

        /// <summary>
        /// Gets a short, one-liner description of the command
        /// </summary>
        public string Description
        {
            get { return "packs a built product"; }
        }

        /// <summary>
        /// Gets a detailed, multiline description of the command and its possible parameters, usage examples
        /// </summary>
        public string Help
        {
            get
            {
                return
@"=Pack command=

Creates a package from a *previously built product*.
Example: `bari pack ExampleProduct`

The product must be built separately!
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

        public PackCommand(IProductPackagerFactory productPackagerFactory, [TargetRoot] IFileSystemDirectory targetRoot)
        {
            this.productPackagerFactory = productPackagerFactory;
            this.targetRoot = targetRoot;
        }

        /// <summary>
        /// Runs the command
        /// </summary>
        /// <param name="suite">The current suite model the command is applied to</param>
        /// <param name="parameters">Parameters given to the command (in unprocessed form)</param>
        /// <returns>Returns <c>true</c> if the command succeeded</returns>
        public bool Run(Suite suite, string[] parameters)
        {
            if (parameters.Length == 1)
                PackProduct(suite, parameters[0]);
            else
                throw new InvalidCommandParameterException("pack",
                                                           "The 'pack' command must be called with a product name!");

            return true;
        }

        private void PackProduct(Suite suite, string productName)
        {
            if (suite.HasProduct(productName))
            {
                var product = suite.GetProduct(productName);
                if (product.Packager != null)
                {
                    var packager = productPackagerFactory.CreateProductPackager(product.Packager.PackagerType);
                    packager.Pack(product, GetProductOutputs(product));
                }
                else
                {
                    throw new InvalidCommandParameterException("pack", "The product has no packager definition!");
                }
            }
            else
            {
                throw new InvalidCommandParameterException("pack", String.Format("Product {0} is not defined!", productName));
            }
        }

        private ISet<TargetRelativePath> GetProductOutputs(Product product)
        {
            var root = targetRoot.GetChildDirectory(product.Name);
            var result = new HashSet<TargetRelativePath>();

            CollectOutput(root, product.Name, root, result);
            return result;
        }

        private void CollectOutput(IFileSystemDirectory productRoot, string productName, IFileSystemDirectory dir, HashSet<TargetRelativePath> result)
        {
            foreach (var file in dir.Files)
            {
                result.Add(new TargetRelativePath(productName, Path.Combine(productRoot.GetRelativePath(dir), file)));
            }

            foreach (var childDir in dir.ChildDirectories)
                CollectOutput(productRoot, productName, dir.GetChildDirectory(childDir), result);
        }
    }
}