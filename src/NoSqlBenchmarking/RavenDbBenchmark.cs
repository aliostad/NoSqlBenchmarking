using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client.Document;

namespace NoSqlBenchmarking
{
    class RavenDbBenchmark : IBenchmarkDummy
    {

		private const string ServerUrl = "http://localhost:8080";
    	private DocumentStore _store;

        public void Init()
        {
			_store = new DocumentStore(){Url = ServerUrl};
        	_store.Initialize();
        }

        public void Save(Dummy dummy)
        {
			
			using (var session = _store.OpenSession())
			{
				session.Store(dummy);
				session.SaveChanges();
			}
        }

        public Dummy Get(string id)
        {		
			_store.Initialize();
			using (var session = _store.OpenSession())
			{
				return session.Load<Dummy>(id);				
			}
        }

        public void Cleanup()
        {
            _store.Dispose();
        }
    }

	class RavenDbBenchmark<TEntity> : IBenchmark<TEntity>
		where TEntity : class , IEntity
	{

		private const string ServerUrl = "http://localhost:8080";
		private DocumentStore _store;

		public void Init()
		{
			_store = new DocumentStore() { Url = ServerUrl };
			_store.Initialize();
		}

		public void Save(TEntity e)
		{
			using (var session = _store.OpenSession())
			{
				session.Store(e);
				session.SaveChanges();
			}
		}

		TEntity IBenchmark<TEntity>.Get(string id)
		{
			_store.Initialize();
			using (var session = _store.OpenSession())
			{
				return session.Load<TEntity>(id);
			}
		}

		public void Cleanup()
		{
			_store.Dispose();
		}
	}

}
