using System.Diagnostics.Contracts;

namespace Bari.Core.UI
{
    /// <summary>
    /// Generic interface for displaying output to the user
    /// </summary>
    [ContractClass(typeof(IUserOutputContracts))]
    public interface IUserOutput
    {
        /// <summary>
        /// Outputs a message to the user. The message can be single or multiline.
        /// 
        /// <para>
        /// The following formatting options must be supported:
        /// <list>
        ///     <item>*Emphasis*</item>
        ///     <item>`command line or code example`</item>
        ///     <item>=Heading=</item>
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="message">The message to be shown</param>
        void Message(string message);

        /// <summary>
        /// Describe an item with a description for the user.
        /// </summary>
        /// <param name="target">The concept to describe</param>
        /// <param name="description">The description</param>
        void Describe(string target, string description);
    }

    /// <summary>
    /// Contracts for the <see cref="IUserOutput"/> interface
    /// </summary>
    [ContractClassFor(typeof(IUserOutput))]
    public abstract class IUserOutputContracts: IUserOutput
    {
        /// <summary>
        /// Outputs a message to the user. The message can be single or multiline.
        /// 
        /// <para>
        /// The following formatting options must be supported:
        /// <list>
        ///     <item>*Emphasis*</item>
        ///     <item>`command line or code example`</item>
        ///     <item>=Heading=</item>
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="message">The message to be shown</param>
        public void Message(string message)
        {
            Contract.Requires(message != null);
        }

        /// <summary>
        /// Describe an item with a description for the user.
        /// </summary>
        /// <param name="target">The concept to describe</param>
        /// <param name="description">The description</param>
        public void Describe(string target, string description)
        {
            Contract.Requires(target != null);
            Contract.Requires(description != null);
        }
    }
}