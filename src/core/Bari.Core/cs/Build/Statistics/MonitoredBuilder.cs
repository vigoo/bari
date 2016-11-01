using System;
using System.Collections.Generic;
using Bari.Core.Generic;
using System.Diagnostics;
using Bari.Core.Build.Cache;

namespace Bari.Core.Build.Statistics
{
	public class MonitoredBuilder: IBuilder
	{
		private readonly IBuilder wrappedBuilder;
		private readonly IBuilderStatistics statistics;
		private readonly Stopwatch stopwatch = new Stopwatch();

		public MonitoredBuilder(IBuilder wrappedBuilder, IBuilderStatistics statistics)
		{
			this.wrappedBuilder = wrappedBuilder;
			this.statistics = statistics;
		}

		public ISet<TargetRelativePath> Run(IBuildContext context)
		{
			stopwatch.Restart();
			try 
			{
				return wrappedBuilder.Run(context);
			}
			finally 
			{
				stopwatch.Stop();
				statistics.Add(MonitoredType, wrappedBuilder.ToString(), stopwatch.Elapsed);
			}
		}

		public bool CanRun()
		{
			return wrappedBuilder.CanRun();
		}

		public IDependencies Dependencies 
		{
			get 
			{
				return wrappedBuilder.Dependencies;
			}
		}

	    public IEnumerable<IBuilder> Prerequisites
	    {
	        get { return wrappedBuilder.Prerequisites; }
	    }

	    public string Uid 
		{
			get 
			{
				return wrappedBuilder.Uid;
			}
		}

		public Type BuilderType
		{
			get
			{
				return wrappedBuilder.BuilderType;
			}
		}

	    public void AddPrerequisite(IBuilder target)
	    {
	        wrappedBuilder.AddPrerequisite(target);
	    }

	    public void RemovePrerequisite(IBuilder target)
	    {
	        wrappedBuilder.RemovePrerequisite(target);
	    }

	    public Type MonitoredType
		{
			get
			{
				if (wrappedBuilder is CachedBuilder)
				{
					return typeof(Cached<>).MakeGenericType(new[] { BuilderType });
				} 
				else
				{
					return BuilderType;
				}
			}
		}

        public BuilderName Name
        {
            get
            {
                return wrappedBuilder.Name;
            }
        }

        public override string ToString()
		{
			return wrappedBuilder.ToString();
		}
	}
}

