using Bari.Core.Model;

namespace Bari.Plugins.FSRepository.Model
{
    public static class SuiteExtensions
    {
        /// <summary>
        /// Gets the <see cref="RepositoryPatternCollection"/> defined in the suite
        /// </summary>
        /// <param name="suite">The suite</param>
        /// <returns>Always returns a valid pattern collection, but it can be empty</returns>
         public static RepositoryPatternCollection GetFSRepositories(this Suite suite)
         {
             if (suite.HasParameters("fs-repositories"))
             {
                 return suite.GetParameters<RepositoryPatternCollection>("fs-repositories");
             }
             else
             {
                 return RepositoryPatternCollection.Empty;
             }
         }
    }
}