using Bari.Core.Exceptions;

namespace Bari.Core.Commands.Clean
{
    public class CleanParameters: ICleanParameters
    {
        private readonly bool keepReferences;

        public CleanParameters(string[] parameters)
        {
            if (parameters.Length == 0)
            {
                keepReferences = false;
            }
            else if (parameters.Length == 1)
            {
                keepReferences = IsKeepReferencesParameter(parameters[0]);
            }
            else
            {
                throw new InvalidCommandParameterException("clean",
                                                           "The 'clean' command must be called with zero or one parameters!");
            }
        }

        public bool KeepReferences
        {
            get { return keepReferences; }
        }

        public bool IsKeepReferencesParameter(string parameter)
        {
            return parameter == "--keep-references" ||
                   parameter == "--keep-refs";
        }
    }
}