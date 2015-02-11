using System;

namespace Bari.Core.Exceptions
{
    public class SuiteValidatorException: Exception
    {
        public SuiteValidatorException(string message) : base(message)
        {
        }

        public override string ToString()
        {
            return String.Format("Suite is invalid: " + Message);
        }
    }
}