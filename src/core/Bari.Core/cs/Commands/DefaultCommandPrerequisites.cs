namespace Bari.Core.Commands
{
    public class DefaultCommandPrerequisites : ICommandPrerequisites
    {
        public bool RequiresSuite
        {
            get { return true; }
        }
    }
}