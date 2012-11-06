using System;

namespace Bari.Core.Exceptions
{
    public class InvalidReferenceException: Exception
    {
        public InvalidReferenceException(string message) : base(message)
        {
        }

        public override string ToString()
        {
            return Message;
        }
    }
}