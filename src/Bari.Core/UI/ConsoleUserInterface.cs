using System;

namespace Bari.Core.UI
{
    /// <summary>
    /// The default command line user interface implementation
    /// </summary>
    public class ConsoleUserInterface: IUserOutput
    {
        /// <summary>
        /// Outputs a message to the user. The message can be single or multiline.
        /// </summary>
        /// <param name="message">The message to be shown</param>
        public void Message(string message)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(message);
        }

        /// <summary>
        /// Describe an item with a description for the user.
        /// </summary>
        /// <param name="target">The concept to describe</param>
        /// <param name="description">The description</param>
        public void Describe(string target, string description)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("    ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(target);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(" - " + description);
        }
    }
}