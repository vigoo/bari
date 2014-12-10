using System;
using System.Collections.Generic;

namespace Bari.Core.Build.Statistics
{
	public class BuilderStats
	{
		public struct Record
		{
			public readonly string Id;
			public readonly TimeSpan Length;

			public Record(string id, TimeSpan length) 
			{
				Id = id;
				Length = length;
			}
		}

		private readonly Type builderType;
		private readonly IList<Record> records = new List<Record>();
		private int maxIndex = -1;
		private int minIndex = -1;
		private TimeSpan sum = TimeSpan.Zero;

		public Type BuilderType { get { return builderType; } }

		public TimeSpan Average { get { return new TimeSpan(sum.Ticks / records.Count); } }
		public Record Min { get { return records[minIndex]; } }
		public Record Max { get { return records[maxIndex]; } }
		public TimeSpan Total { get { return sum; } }
		public int Count { get { return records.Count; } }
		public IEnumerable<Record> All { get { return records; } }

		public BuilderStats(Type builderType)
		{
			this.builderType = builderType;
		}

		public void Add(string description, TimeSpan elapsed)
		{
			if (maxIndex == -1 || records[maxIndex].Length < elapsed)			
				maxIndex = records.Count;
			if (minIndex == -1 || records[minIndex].Length > elapsed)			
				minIndex = records.Count;

			sum += elapsed;

			records.Add(new Record(description, elapsed));		
		}
	}
}

