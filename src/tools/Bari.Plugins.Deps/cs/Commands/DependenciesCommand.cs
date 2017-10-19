using System;
using System.Collections.Generic;
using Bari.Core.Commands;
using Bari.Core.Exceptions;
using Bari.Core.Generic;
using Bari.Core.Generic.Graph;
using Bari.Core.Model;
using QuickGraph;

namespace Bari.Plugins.Deps.Commands
{
    class DependenciesCommand : ICommand
    {
        private readonly IFileSystemDirectory targetRoot;

        public string Name
        {
            get { return "dependencies"; }
        }

        public string Description
        {
            get { return "creates dependency graph"; }
        }

        public string Help
        {
            get
            {
                return
@"=Dependencies command=

When used without parameter, it creates the dependency graph of the full suite. 
Example: `bari dependencies`

When used with a *product* name, it creates the dependency graph of that product.
Example: `bari dependencies exampleProduct`

When the special `--module` argument is specified, the dependency graph will contanins only the modules.
Example: `bari dependencies --module`
Example: `bari dependencies exampleProduct --module`
";
            }
        }

        public bool NeedsExplicitTargetGoal
        {
            get { return true; }
        }

        public DependenciesCommand([TargetRoot] IFileSystemDirectory targetRoot)
        {
            this.targetRoot = targetRoot;
        }

        public bool Run(Core.Model.Suite suite, string[] parameters)
        {
            var effectiveLength = parameters.Length;
            var modulesOnly = false;

            if (effectiveLength > 0)
            {
                modulesOnly = effectiveLength >= 1 && parameters[effectiveLength - 1] == "--module";
            }

            var modules = suite.Modules;
            var target = "suite";

            if ((modulesOnly && effectiveLength > 1) || !(modulesOnly && effectiveLength > 0))
            {
                var product = parameters[0];
                if (suite.HasProduct(product))
                {
                    target = product;
                    modules = suite.GetProduct(product).Modules;
                }
                else
                    throw new InvalidCommandParameterException("dependecies", "The given project does not exist");
            }

            try
            {
                using (var writer = new DotWriter(targetRoot.CreateBinaryFile("deps." + target + ".dot")))
                {
                    var edges = new HashSet<EquatableEdge<string>>();

                    writer.Rankdir = "LR";
                    writer.RemoveSelfLoops = true;

                    foreach (var module in modules)
                    {
                        foreach (var project in module.Projects)
                        {
                            foreach (var reference in project.References)
                            {
                                EquatableEdge<string> edge = null;
                                if (modulesOnly)
                                    edge = GetModuleEdge(reference, project);
                                else
                                    edge = GetProjectEdge(reference, project);

                                if (edge != null)
                                    edges.Add(edge);
                            }
                        }
                    }
                    writer.WriteGraph(edges);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private EquatableEdge<string> GetProjectEdge(Reference reference, Project project)
        {
            switch (reference.Uri.Scheme)
            {
                case "suite":
                    {
                        var projectName = reference.Uri.AbsolutePath.TrimStart('/');
                        return new EquatableEdge<string>(project.Name.ToLowerInvariant(), projectName.ToLowerInvariant());
                    }
                case "module":
                    {
                        var projectName = reference.Uri.Host;
                        var refProject = project.Module.GetProjectOrTestProject(projectName);
                        return new EquatableEdge<string>(project.Name.ToLowerInvariant(), refProject.Name.ToLowerInvariant());
                    }
            }
            return null;
        }

        private EquatableEdge<string> GetModuleEdge(Reference reference, Project project)
        {
            switch (reference.Uri.Scheme)
            {
                case "suite":
                    return new EquatableEdge<string>(project.Module.Name.ToLowerInvariant(), reference.Uri.Host.ToLowerInvariant());
                case "module":
                    var projectName = reference.Uri.Host;
                    var refProject = project.Module.GetProjectOrTestProject(projectName);
                    return new EquatableEdge<string>(project.Module.Name.ToLowerInvariant(), refProject.Module.Name.ToLowerInvariant());
            }
            return null;
        }
    }
}
