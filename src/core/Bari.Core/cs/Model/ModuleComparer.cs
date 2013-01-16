using System;
using System.Collections.Generic;

namespace Bari.Core.Model
{
    public class ModuleComparer: IComparer<Module>
    {
        public int Compare(Module x, Module y)
        {
            return string.Compare(x.Name, y.Name, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}