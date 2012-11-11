using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace NoSqlBenchmarking
{
    class Program
    {
        static void Main(string[] args)
        {
			//Test(new RavenDbBenchmark(), 10000);
			//Test(new MongoDbBenchmark(), 10000);
			//Test(new RedisBenchmark(), 10000);
			Test(new SqlServerBenchmark(), 10000);
			//Test(new CassandraBenchmark(), 10000);
        	Console.Read();
        }

		private static void Test(IBenchmark benchmarkRunner, int n)
		{
			benchmarkRunner.Init();
			var stopwatch = new Stopwatch();
			var random = new Random();
			Console.ForegroundColor = ConsoleColor.DarkGreen;
			Console.WriteLine("Starting benchmark with " + benchmarkRunner.GetType().Name);
			Console.ForegroundColor = ConsoleColor.DarkYellow;
			stopwatch.Start();
			for (int i = 1; i <= n; i++)
			{
				var d = new Dummy()
				        	{
				        		Id = Guid.NewGuid().ToString("N"),
								Blob = new byte[random.Next(4 * 1024, 20 * 1024)] // 4-20 KB
				        	};
				random.NextBytes(d.Blob);
				benchmarkRunner.Save(d);
				var d2 = benchmarkRunner.Get(d.Id);
				if (d2 == null)
					throw new InvalidOperationException("Could not find item");

				var d3 = benchmarkRunner.Get(d.Id + 1);
				if (d3 != null)
					throw new InvalidOperationException("Found a non-existent item");
				if (i % 100 == 0)
					Console.Write("\r" + i);
			}
			stopwatch.Stop();
			Console.WriteLine();
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("Took " + stopwatch.Elapsed);
			Console.WriteLine("---------------------------------------");
			benchmarkRunner.Cleanup();
		}
    }
}
