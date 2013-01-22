using System.Collections.Generic;
using Bari.Core.Model;

namespace Bari.Core.Commands.Helper
{
    public class ModuleTarget: CommandTarget
    {
        private readonly Module module;

        public ModuleTarget(Module module)
        {
            this.module = module;
        }

        public override IEnumerable<Project> Projects
        {
            get { return module.Projects; }
        }

        public override IEnumerable<Project> TestProjects
        {
            get { return module.TestProjects; }
        }
    }
}