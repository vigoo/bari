namespace Bari.Core.Commands
{
    public class HelpCommandPrerequisites : ICommandPrerequisites
    {
        public bool RequiresSuite
        {
            get { return false; }
        }
    }
}