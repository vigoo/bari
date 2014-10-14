namespace Bari.Core.Commands
{
    public class SelfUpdateCommandPrerequisites : ICommandPrerequisites
    {
        public bool RequiresSuite
        {
            get { return false; }
        }
    }
}