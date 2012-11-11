using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace NoSqlBenchmarking
{
    class Program
    {
        static void Main(string[] args)
        {
			Test(new RavenDbBenchmark(), 10000,
				operations: BenchmarkOperation.Insert | BenchmarkOperation.Get | BenchmarkOperation.GetNonExistent,
				gatherStatistics: true);
			//Test(new MongoDbBenchmark(), 10000);
			//Test(new RedisBenchmark(), 10000);
			//Test(new SqlServerBenchmark(), 10000);
			//Test(new NoActionBenchmark(), 100000, gatherStatistics: true);
			//Test(new CassandraBenchmark(), 10000);
        	Console.Read();
        }

		private static void Test(IBenchmark benchmarkRunner, int n,
			BenchmarkOperation operations = BenchmarkOperation.Insert | BenchmarkOperation.Get | BenchmarkOperation.GetNonExistent,
			bool gatherStatistics = false)
		{
			string benchmarker = benchmarkRunner.GetType().Name;
			StreamWriter writer = null;
			double totalMilliseconds = 0;
			if(gatherStatistics)
			{
				writer = new StreamWriter(benchmarker + ".csv");
				writer.Write("n, milliseconds\r\n");
			}
			benchmarkRunner.Init();
			var stopwatch = new Stopwatch();
			var random = new Random();
			Console.ForegroundColor = ConsoleColor.DarkGreen;
			Console.WriteLine("Starting benchmark with " + benchmarker);
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
				if( (operations & BenchmarkOperation.Insert) == BenchmarkOperation.Insert)
				{
					benchmarkRunner.Save(d);					
				}
				if( (operations & BenchmarkOperation.Get) == BenchmarkOperation.Get)
				{
					var d2 = benchmarkRunner.Get(d.Id);
					if (d2 == null)
						throw new InvalidOperationException("Could not find item");
				}

				if ((operations & BenchmarkOperation.GetNonExistent) == BenchmarkOperation.GetNonExistent)
				{
					var d3 = benchmarkRunner.Get(d.Id + 1);
					if (d3 != null)
						throw new InvalidOperationException("Found a non-existent item");
				}
			
				if (i % 100 == 0)
				{
					Console.Write("\r" + i);
					if(gatherStatistics)
					{
						var current = stopwatch.Elapsed.TotalMilliseconds;
						writer.Write(i + ",");
						writer.Write(current - totalMilliseconds);
						writer.Write("\r\n");
						totalMilliseconds = current;
					}
				}

					
			}
			stopwatch.Stop();
			Console.WriteLine();
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("Took " + stopwatch.Elapsed);
			Console.WriteLine("Average " + (stopwatch.Elapsed.TotalMilliseconds / n).ToString("0.0"));
			Console.WriteLine("---------------------------------------");
			benchmarkRunner.Cleanup();
			if (gatherStatistics)
			{
				writer.Flush();
				writer.Close();
			}
		}
    }
}
