using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BookSleeve;

namespace NoSqlBenchmarking
{
	class RedisBenchmark : IBenchmark
	{

		private RedisConnection _connection;
		private const int _dbId = 0;

		public void Init()
		{
			_connection = new RedisConnection("localhost");
			_connection.Open();
		}

		public void Save(Dummy dummy)
		{
			_connection.Strings.Set(_dbId, dummy.Id, dummy.Blob).Wait();
		}

		public Dummy Get(string id)
		{
			try
			{
				var result = _connection.Strings.Get(_dbId, id).Result;
				return result == null ? null : new Dummy()
				{
					Id = id,
					Blob = result
				};

			}
			catch (Exception)
			{
				return null;
			}
				
		}

		public void Cleanup()
		{
			_connection.Close(true);
		}
	}
}
