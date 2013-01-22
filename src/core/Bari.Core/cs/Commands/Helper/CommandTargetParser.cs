using System;
using System.Collections.Generic;
using System.Linq;
using Bari.Core.Model;

namespace Bari.Core.Commands.Helper
{
    public class CommandTargetParser : ICommandTargetParser
    {
        private readonly Suite suite;

        public CommandTargetParser(Suite suite)
        {
            this.suite = suite;
        }

        public CommandTarget ParseTarget(string target)
        {
            if (string.IsNullOrEmpty(target))
                return new FullSuiteTarget(suite);
            else
            {
                if (suite.HasProduct(target))
                {
                    var product = suite.GetProduct(target);
                    return new ProductTarget(product);
                }
                if (suite.HasModule(target))
                {
                    var module = suite.GetModule(target);
                    return new ModuleTarget(module);
                }
                else
                {
                    var matches = new HashSet<Project>();

                    // Looking for modulename.projectname matches
                    foreach (var module in suite.Modules)
                    {
                        if (target.StartsWith(module.Name + '.', StringComparison.InvariantCultureIgnoreCase))
                        {
                            string projectName = target.Substring(module.Name.Length + 1);
                            if (module.HasProject(projectName))
                                matches.Add(module.GetProject(projectName));
                        }
                    }

                    // If there is only one match
                    if (matches.Count == 1)
                    {
                        var project = matches.First();

                        return new ProjectTarget(project);
                    }
                    else
                    {
                        if (matches.Count > 1)
                            throw new ArgumentException(
                                "The given module and project name identifies more than one projects", "target");
                        else
                            throw new ArgumentException("The given project does not exist", "target");
                    }
                }
            }
        }
    }
}