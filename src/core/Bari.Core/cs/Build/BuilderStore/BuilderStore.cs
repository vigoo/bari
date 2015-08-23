using System.Collections.Generic;
using Bari.Core.UI;

namespace Bari.Core.Build.BuilderStore
{
    public class BuilderStore: IBuilderStore
    {
        private readonly IDictionary<IBuilder, IBuilder> instances = new Dictionary<IBuilder, IBuilder>();
        private int addCount = 0;

        public T Add<T>(T builder)
            where T: IBuilder
        {
            addCount++;
            if (instances.ContainsKey(builder))
            {
                return (T)instances[builder];
            }
            else
            {
                instances.Add(builder, builder);
                return builder;
            }
        }

        public void DumpStats(IUserOutput output)
        {
            output.Message("Builder store statistics");
            output.Indent();
            output.Message("Total number of created builders: {0}", addCount);
            output.Message("Number of unique builders: {0}", instances.Count);
            output.Unindent();
        }
    }
}