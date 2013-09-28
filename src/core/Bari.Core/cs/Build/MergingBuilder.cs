using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bari.Core.Build.Dependencies;
using Bari.Core.Generic;

namespace Bari.Core.Build
{
    public class MergingBuilder: IBuilder
    {
        private readonly ISet<IBuilder> sourceBuilders;

        public MergingBuilder(IEnumerable<IBuilder> sourceBuilders)
        {
            this.sourceBuilders = new HashSet<IBuilder>(sourceBuilders);
        }

        /// <summary>
        /// Dependencies required for running this builder
        /// </summary>
        public IDependencies Dependencies
        {
            get { return new MultipleDependencies(sourceBuilders.Select(b => b.Dependencies)); }
        }

        /// <summary>
        /// Gets an unique identifier which can be used to identify cached results
        /// </summary>
        public string Uid
        {
            get
            {
                var uids = new StringBuilder();
                foreach (var builder in sourceBuilders)
                    uids.AppendLine(builder.Uid);

                return MD5.Encode(uids.ToString());
            }
        }

        /// <summary>
        /// Prepares a builder to be ran in a given build context.
        /// 
        /// <para>This is the place where a builder can add additional dependencies.</para>
        /// </summary>
        /// <param name="context">The current build context</param>
        public void AddToContext(IBuildContext context)
        {
            foreach (var builder in sourceBuilders)
                builder.AddToContext(context);

            context.AddBuilder(this, sourceBuilders);
        }

        /// <summary>
        /// Runs this builder
        /// </summary>
        /// <param name="context">Current build context</param>
        /// <returns>Returns a set of generated files, in target relative paths</returns>
        public ISet<TargetRelativePath> Run(IBuildContext context)
        {
            var result = new HashSet<TargetRelativePath>();

            foreach (var builder in sourceBuilders)
                result.UnionWith(context.GetResults(builder));

            return result;
        }

        public override string ToString()
        {
            return "[Merge]";
        }
    }
}