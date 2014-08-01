using System;
using System.Collections.Generic;

namespace Bari.Core.Build
{
    public interface IBuilderEnumerator
    {
        IEnumerable<Type> GetAllPersistentBuilders();
    }
}