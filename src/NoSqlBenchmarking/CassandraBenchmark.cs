using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentCassandra;

namespace NoSqlBenchmarking
{
	class CassandraBenchmark : IBenchmark
	{
		private const string DummiesKeySpace = "DummiesKeyspace2";

		private CassandraContext _context;
		private CassandraColumnFamily _dummies;


		public void Init()
		{
			_context = new CassandraContext(keyspace: DummiesKeySpace, host: "localhost");
			if (!_context.KeyspaceExists(DummiesKeySpace))
			{
				var keyspace = new CassandraKeyspace(new CassandraKeyspaceSchema
				{
					Name = DummiesKeySpace,
				}, _context);
				keyspace.TryCreateSelf();
				//_context.DropColumnFamily("Dummies");
				_context.ExecuteQuery(
					@"
                CREATE COLUMNFAMILY Dummies (
	            KEY ascii PRIMARY KEY,
	            Blob blob
                );");
				_context.SaveChanges();
			}
			
			_dummies = _context.GetColumnFamily("Dummies");

		}

		public void Save(Dummy dummy)
		{
			dynamic d = _dummies.CreateRecord(dummy.Id);
			d.Blob = dummy.Blob;
			_context.Attach(d);
			_context.SaveChanges();

		}

		public Dummy Get(string id)
		{
			
			dynamic dummy = _dummies.Get(id).FirstOrDefault();
			return ((IList<FluentColumn>)dummy.Columns).Count == 0?
				null :
				new Dummy()
					{
						Id = id,
						Blob = dummy.Blob
					};
		}

		public void Cleanup()
		{
			_context.Dispose();
		}
	}
}
