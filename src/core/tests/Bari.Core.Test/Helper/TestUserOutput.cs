using System;
using System.Collections.Generic;
using Bari.Core.UI;

namespace Bari.Core.Test.Helper
{
    public class TestUserOutput: IUserOutput
    {
        private readonly IList<string> messages = new List<string>();
        private readonly IList<Tuple<string, string>> descriptions = new List<Tuple<string, string>>();

        public IList<string> Messages
        {
            get { return messages; }
        }

        public IList<Tuple<string, string>> Descriptions
        {
            get { return descriptions; }
        }

        public void Reset()
        {
            messages.Clear();
            descriptions.Clear();
        }

        /// <summary>
        /// Outputs a message to the user. The message can be single or multiline.
        /// </summary>
        /// <param name="message">The message to be shown</param>
        public void Message(string message)
        {
            messages.Add(message);
        }

        /// <summary>
        /// Describe an item with a description for the user.
        /// </summary>
        /// <param name="target">The concept to describe</param>
        /// <param name="description">The description</param>
        public void Describe(string target, string description)
        {
            descriptions.Add(Tuple.Create(target, description));
        }
    }
}