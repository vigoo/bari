using System.Linq;
using Bari.Core.Exceptions;

namespace Bari.Core.Commands.Clean
{
    public class CleanParameters: ICleanParameters
    {
        private readonly bool keepReferences;
        private readonly bool softClean;

        public CleanParameters(string[] parameters)
        {
            if (parameters.Length <= 2)
            {
                keepReferences = parameters.Any(IsKeepReferencesParameter);
                softClean = parameters.Any(IsSoftCleanParameter);
            }
            else
            {
                throw new InvalidCommandParameterException("clean",
                    "The 'clean' command must be called with zero, one or two parameters!");
            }
        }

        public bool KeepReferences
        {
            get { return keepReferences; }
        }

        public bool SoftClean
        {
            get { return softClean; }
        }

        public bool IsKeepReferencesParameter(string parameter)
        {
            return parameter == "--keep-references" ||
                   parameter == "--keep-refs";
        }

        public bool IsSoftCleanParameter(string parameter)
        {
            return parameter == "--soft-clean";
        }
    }
}