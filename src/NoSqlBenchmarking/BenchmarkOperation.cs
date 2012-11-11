using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NoSqlBenchmarking
{
	[Flags]
	internal enum BenchmarkOperation
	{
		None = 0,
		Insert = 1,
		Get = 2,
		GetNonExistent = 4,
		Update = 8
	} 
}
