using System;
using System.Collections.Generic;
using System.Linq;

namespace Bari.Core.Build.Statistics
{
	public class DefaultBuilderStatistics: IBuilderStatistics
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(DefaultBuilderStatistics));
		private readonly IDictionary<Type, BuilderStats> builderStats = new Dictionary<Type, BuilderStats>();

		public void Add(Type builderType, string description, TimeSpan elapsed)
		{
			BuilderStats stats;
			if (!builderStats.TryGetValue(builderType, out stats))
			{
				stats = new BuilderStats(builderType);
				builderStats.Add(builderType, stats);
			}

			stats.Add(description, elapsed);
		}

		public void Dump()
		{
			log.Debug("Builder performance statistics");
			log.Debug("----");

			var byTotal = builderStats.OrderByDescending(kv => kv.Value.Total);
			foreach (var item in byTotal)
			{
				log.DebugFormat("# {0} ({1}x) => total: {1:F}s, average: {2:F}s", item.Key.Name, item.Value.Count, item.Value.Total.TotalSeconds, item.Value.Average.TotalSeconds);

				var records = item.Value.All.OrderByDescending(r => r.Length);
				foreach (var record in records)
				{
					log.DebugFormat("    - {0}: {1:F}s", record.Id, record.Length);
				}
			}

			log.Debug("----");
		}
	}
}

