using Bari.Core.Model.Parameters;

namespace Bari.Core.Model
{
    public class PostProcessDefinition
    {
        private readonly string name;
        private readonly PostProcessorId postProcessorId;
        private IProjectParameters parameters;

        public string Name
        {
            get { return name; }
        }

        public PostProcessorId PostProcessorId
        {
            get { return postProcessorId; }
        }

        public IProjectParameters Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }

        public PostProcessDefinition(string name, PostProcessorId postProcessorId)
        {
            this.name = name;
            this.postProcessorId = postProcessorId;
        }
    }
}