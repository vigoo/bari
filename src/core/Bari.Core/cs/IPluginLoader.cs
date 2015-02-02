using System;
using Ninject.Modules;

namespace Bari.Core
{
    public interface IPluginLoader
    {
        void Load(Uri referenceUri);

        void Load(string path);

        void Load(INinjectModule module);
    }
}