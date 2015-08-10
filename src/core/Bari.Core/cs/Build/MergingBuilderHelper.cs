namespace Bari.Core.Build
{
    /// <summary>
    /// Helper extension methods to create <see cref="MergingBuilder"/> instances
    /// </summary>
    public static class MergingBuilderHelper
    {
        /// <summary>
        /// Merges a set of builders in an efficient way
        /// </summary>
        /// <param name="coreBuilderFactory">Core builder factory to create the new builder if necessary</param>
        /// <param name="builders">Builders to merge</param>
        /// <returns>Returns <c>null</c> if the source array was empty, a single builder if it had only one element
        /// and a new merging builder if it had more than one elements</returns>
        public static IBuilder Merge(this ICoreBuilderFactory coreBuilderFactory, IBuilder[] builders)
         {
             if (builders.Length == 0)
                 return null;
             else if (builders.Length == 1)
                 return builders[0];
             else
                 return coreBuilderFactory.CreateMergingBuilder(builders);
         }
    }
}