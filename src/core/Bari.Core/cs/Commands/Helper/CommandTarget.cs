using System.Collections.Generic;
using Bari.Core.Model;

namespace Bari.Core.Commands.Helper
{
    public abstract class CommandTarget
    {
        public abstract IEnumerable<Project> Projects { get; }
        public abstract IEnumerable<TestProject> TestProjects { get; }
    }
}