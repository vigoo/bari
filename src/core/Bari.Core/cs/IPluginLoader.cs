using System;

namespace Bari.Core
{
    public interface IPluginLoader
    {
        void Load(Uri referenceUri);
    }
}