using System;

namespace Bari.Core.Build.Statistics
{
	public interface IBuilderStatistics
	{
		void Add(Type builderType, string description, TimeSpan elapsed);
		void Dump();
	}
}

