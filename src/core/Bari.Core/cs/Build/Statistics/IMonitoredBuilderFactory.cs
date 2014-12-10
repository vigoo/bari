using System;

namespace Bari.Core.Build.Statistics
{
	public interface IMonitoredBuilderFactory
	{
		MonitoredBuilder CreateMonitoredBuilder(IBuilder wrappedBuilder, IBuilderStatistics statistics);
	}
}

