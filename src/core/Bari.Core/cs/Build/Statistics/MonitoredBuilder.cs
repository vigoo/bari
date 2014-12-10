using System;
using Bari.Core.Build;
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

		public void AddToContext(IBuildContext context)
		{
			wrappedBuilder.AddToContext(context);
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

		public override string ToString()
		{
			return wrappedBuilder.ToString();
		}
	}
}

