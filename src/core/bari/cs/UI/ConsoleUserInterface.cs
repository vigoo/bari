using System;
using Bari.Core.UI;

namespace Bari.Console.UI
{
    /// <summary>
    /// The default command line user interface implementation
    /// </summary>
    public class ConsoleUserInterface: IUserOutput
    {
        private int indent;

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
            System.Console.ForegroundColor = ConsoleColor.Gray;

            bool inEmphasis = false;
            bool inExample = false;
            bool inHeading = false;
            bool isEscaping = false;

            System.Console.Write(IndentString);

            foreach (var ch in message)
            {
                if (ch == '*' && !isEscaping)
                {
                    inEmphasis = !inEmphasis;
                    ChangeColor(inEmphasis, inExample, inHeading);
                }
                else if (ch == '`' && !isEscaping)
                {
                    inExample = !inExample;
                    ChangeColor(inEmphasis, inExample, inHeading);
                }
                else if (ch == '=' && !isEscaping)
                {
                    inHeading = !inHeading;
                    ChangeColor(inEmphasis, inExample, inHeading);
                }
                else
                {
                    System.Console.Write(ch);
                }

                isEscaping = ch == '\\';
            }

            System.Console.ForegroundColor = ConsoleColor.Gray;
            System.Console.WriteLine();
        }

        private void ChangeColor(bool inEmphasis, bool inExample, bool inHeading)
        {
            if (inHeading)
            {
                System.Console.ForegroundColor = ConsoleColor.Green;
            }
            else
            {
                if (inEmphasis)
                    System.Console.ForegroundColor = ConsoleColor.Yellow;
                else if (inExample)
                    System.Console.ForegroundColor = ConsoleColor.White;
                else
                    System.Console.ForegroundColor = ConsoleColor.Gray;
            }
        }

        /// <summary>
        /// Describe an item with a description for the user.
        /// </summary>
        /// <param name="target">The concept to describe</param>
        /// <param name="description">The description</param>
        public void Describe(string target, string description)
        {
            System.Console.ForegroundColor = ConsoleColor.Gray;
            System.Console.Write(IndentString + "    ");
            System.Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.Write(target);
            System.Console.ForegroundColor = ConsoleColor.Gray;
            System.Console.WriteLine(" - " + description);
        }

        /// <summary>
        /// Shows a warning message
        /// </summary>
        /// <param name="message">The message to be shown</param>
        /// <param name="hints">Optional hints about the warning</param>
        public void Warning(string message, string[] hints = null)
        {
            System.Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine(IndentString + "Warning: {0}", message);

            if (hints != null && hints.Length > 0)
            {
                System.Console.ForegroundColor = ConsoleColor.DarkYellow;
                System.Console.WriteLine("\n" + IndentString + "Hints:");

                Indent();
                foreach (var hint in hints)
                {
                    System.Console.WriteLine(IndentString + "- {0}", hint);
                }
                Unindent();
            }

            System.Console.ForegroundColor = ConsoleColor.Gray;
        }

        /// <summary>
        /// Shows an error message
        /// </summary>
        /// <param name="message">Error message</param>
        public void Error(string message)
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine(IndentString + message);
            System.Console.ForegroundColor = ConsoleColor.Gray;
        }

        public void Indent()
        {
            indent += 1;
        }

        public void Unindent()
        {
            if (indent == 0)
                throw new InvalidOperationException();

            indent -= 1;
        }

        private string IndentString
        {
            get
            {
                return new String(' ', indent*2);
            }
        }
    }
}