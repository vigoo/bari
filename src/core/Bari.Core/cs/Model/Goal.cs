namespace Bari.Core.Model
{
    public class Goal
    {
        private readonly string name;

        public string Name
        {
            get { return name; }
        }

        public Goal(string name)
        {
            this.name = name;
        }
    }
}