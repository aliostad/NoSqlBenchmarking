using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace NoSqlBenchmarking
{
    class Program
    {

        private static int currentCount = 0;
        private static int failedCount = 0;

        static void Main(string[] args)
        {
        	while (true)
        	{
				int store = TakeUserInput(@"Please choose a store: 
1) Cassandra
2) MongoDB
3) RavenDB
4) Redis
5) SQL Server
", 1);
				IBenchmark benchmark = null;
				switch (store)
				{
					case 1:
						benchmark = new CassandraBenchmark();
						break;
					case 2:
						benchmark = new MongoDbBenchmark();
						break;
					case 3:
						benchmark = new RavenDbBenchmark();
						break;
					case 4:
						benchmark = new RedisBenchmark();
						break;
					case 5:
						benchmark = new SqlServerBenchmark();
						break;
					default:
						benchmark = new CassandraBenchmark();
						break;
				}

				int n = TakeUserInput("Please enter number of operations (enter empty for 10000 operations): ", 10000);
                int gatherStats = TakeUserInput("Gather statistics every 100 operations? (enter 1 if you want stats)", 0);
                int concurrency = TakeUserInput("Run multi-threaded? in which case enter number of threads", 0);

				try
				{
                    
					Test(benchmark, n,
					    operations: BenchmarkOperation.Insert | BenchmarkOperation.Get | BenchmarkOperation.GetNonExistent,
					    gatherStatistics: gatherStats == 1, concurrency: concurrency);
                    

				}
				catch (Exception e)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine(e);
				}

				Console.ForegroundColor = ConsoleColor.DarkMagenta;
				Console.WriteLine("Press <ENTER> to exit, c to continue");
        		var read = Console.ReadKey();
				Console.WriteLine();
				if(read.KeyChar!='c')
					break;
        	}

        }

		private static int TakeUserInput(string message, int defaultValue = 0)
		{
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.Write(message);
			Console.ForegroundColor = ConsoleColor.Green;
			var line = Console.ReadLine();
			int i = 0;
			if(Int32.TryParse(line, out i))
			{
				return i;
			}
			else
			{
				return defaultValue;
			}

		}

		private static void Test(IBenchmark benchmarkRunner, int n,
			BenchmarkOperation operations = BenchmarkOperation.Insert | BenchmarkOperation.Get | BenchmarkOperation.GetNonExistent,
			bool gatherStatistics = false, int concurrency = 50)
		{
			string benchmarker = benchmarkRunner.GetType().Name;
			StreamWriter writer = null;
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

            if(concurrency>0)
                WriteReadNTimesAsync(n, benchmarkRunner, operations, gatherStatistics, stopwatch, writer, concurrency);
            else
                WriteReadNTimes(n, benchmarkRunner, operations, gatherStatistics, stopwatch, writer);


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

        private static void WriteReadNTimesAsync(int n, IBenchmark benchmarkRunner,
            BenchmarkOperation operations, bool gatherStatistics, 
            Stopwatch stopwatch, StreamWriter writer, int concurrency
            )
        {
            var random = new Random();
            double totalMilliseconds = 0;

            ThreadPool.SetMinThreads(concurrency, 100);
            ThreadPool.SetMaxThreads(concurrency, 100);

            Console.Write("Warming up");
            for (int i = 0; i < concurrency; i++)
            {
                ThreadPool.QueueUserWorkItem((o) => Console.Write("."));
            }
            Thread.Sleep(2000);

            Console.WriteLine("it should be OK now.");




            for (int i = 1; i <= n; i++)
            {
                WriteReadDummy(benchmarkRunner, operations, random);


                if (i % 100 == 0)
                {
                    Console.Write("\r" + i);
                    if (gatherStatistics)
                    {
                        var current = stopwatch.Elapsed.TotalMilliseconds;
                        writer.Write(i + ",");
                        writer.Write(current - totalMilliseconds);
                        writer.Write("\r\n");
                        totalMilliseconds = current;
                    }
                }


            }
        }

        private static void WriteReadNTimes(int n, IBenchmark benchmarkRunner,
       BenchmarkOperation operations, bool gatherStatistics,
       Stopwatch stopwatch, StreamWriter writer
       )
        {
            var random = new Random();
            double totalMilliseconds = 0;
            failedCount = 0;
            currentCount = 0;
            var resetEvent = new AutoResetEvent(false);

            for (int i = 0; i < n; i++)
            {
                ThreadPool.QueueUserWorkItem(
                    (o) =>
                        {
                            try
                            {
                                WriteReadDummy(benchmarkRunner, operations, random);
                                Interlocked.Increment(ref currentCount);
                            }
                            catch
                            {
                                Interlocked.Increment(ref failedCount);
                            }
                        }
                    );
            }

            while (true)
            {

                resetEvent.WaitOne(1000);
                Console.Write("\r" + currentCount);

                if(currentCount == n)
                    break;
            }

            Console.WriteLine("Failed count: " + failedCount);
            
        }

        private static void WriteReadDummy(IBenchmark benchmarkRunner, BenchmarkOperation operations, Random random)
        {

            var d = new Dummy()
            {
                Id = Guid.NewGuid().ToString("N"),
                Blob = new byte[random.Next(4 * 1024, 20 * 1024)] // 4-20 KB
            };

            random.NextBytes(d.Blob);
            if ((operations & BenchmarkOperation.Insert) == BenchmarkOperation.Insert)
            {
                benchmarkRunner.Save(d);
            }
            if ((operations & BenchmarkOperation.Get) == BenchmarkOperation.Get)
            {
                var d2 = benchmarkRunner.Get(d.Id);
                if (d2 == null)
                    throw new InvalidOperationException("Could not find item");
                if (d2.Id != d.Id)
                    throw new InvalidOperationException("Has a different id: " + d2.Id);

            }

            if ((operations & BenchmarkOperation.GetNonExistent) == BenchmarkOperation.GetNonExistent)
            {
                var d3 = benchmarkRunner.Get(d.Id + 1);
                if (d3 != null)
                    throw new InvalidOperationException("Found a non-existent item");
            }
        }
    }
}
