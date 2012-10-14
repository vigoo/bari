using Bari.Core.Build.Cache;
using Ninject;
using Ninject.Parameters;
using Ninject.Syntax;

namespace Bari.Core.Build
{
    /// <summary>
    /// Helper class for automatically wrapping <see cref="IBuilder"/> instances with a 
    /// <see cref="CachedBuilder"/>.
    /// </summary>
    public static class BuilderResolver
    {
        /// <summary>
        /// Creates a new builder instance and wraps it with <see cref="CachedBuilder"/> automatically.
        /// </summary>
        /// <typeparam name="T">Builder type to be created</typeparam>
        /// <param name="root">Interface to create new instances</param>
        /// <param name="parameters">Optional parameters for the inner builder</param>
        /// <returns>Returns a cached builder instance</returns>
         public static IBuilder GetBuilder<T>(this IResolutionRoot root, params IParameter[] parameters)
             where T: IBuilder
         {
             var builder = root.Get<T>(parameters);
             var cachedBuilder = root.Get<CachedBuilder>(new ConstructorArgument("wrappedBuilder", builder));
             return cachedBuilder;
         }
    }
}