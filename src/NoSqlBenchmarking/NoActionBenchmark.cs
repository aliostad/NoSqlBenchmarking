using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NoSqlBenchmarking
{
	class NoActionBenchmark : IBenchmarkDummy
	{
		public void Init()
		{
			
		}

		public void Save(Dummy dummy)
		{
			
		}

		public Dummy Get(string id)
		{
			return id.Length%2 == 0
			       	? new Dummy()
			       	: null;

		}

		public void Cleanup()
		{
			
		}
	}
}
