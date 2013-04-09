using System.Collections.Generic;

namespace Bari.Core.Commands
{
    public interface ICommandEnumerator
    {
        IEnumerable<string> AvailableCommands { get; } 
    }
}