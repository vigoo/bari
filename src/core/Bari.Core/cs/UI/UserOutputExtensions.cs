using System.Diagnostics.Contracts;

namespace Bari.Core.UI
{
    /// <summary>
    /// Extension methods to the <see cref="IUserOutput"/> interface to make
    /// its use more convenient
    /// </summary>
    public static class UserOutputExtensions
    {
        /// <summary>
        /// Formatted message output
        /// </summary>
        /// <param name="output">Output interface to be used</param>
        /// <param name="format">Format string, used by <see cref="string.Format(string,object)"/></param>
        /// <param name="args">Parameters of the string formatting</param>
         public static void Message(this IUserOutput output, string format, params object[] args)
         {
             Contract.Requires(output != null);
             Contract.Requires(format != null);
             Contract.Requires(args != null);

             output.Message(string.Format(format, args));
         }
    }
}