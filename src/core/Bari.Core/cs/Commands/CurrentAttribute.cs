using System;

namespace Bari.Core.Commands
{
    /// <summary>
    /// Attribute indicating that a parameter of type <see cref="ICommand"/>
    /// should be bound to currently running command
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class CurrentAttribute: Attribute
    {
         
    }
}