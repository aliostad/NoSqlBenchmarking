using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Norm;
using Norm.Collections;

namespace NoSqlBenchmarking
{
	public class MongoDbBenchmark : IBenchmarkDummy
	{

		private IMongo _store;
		private IMongoCollection<Dummy> _collection;
		public void Init()
		{
			_store = Mongo.Create("mongodb://localhost/test");
			_collection = _store.GetCollection<Dummy>();
		}

		public void Save(Dummy dummy)
		{
			_collection.Save(dummy);
		}

		public Dummy Get(string id)
		{
			return _collection.FindOne(new {Id = id});
		}

		public void Cleanup()
		{
			_store.Dispose();
		}
	}

	class MongoDbBenchmark<TEntity> : IBenchmark<TEntity>
		where TEntity : class , IEntity
	{

		private IMongo _store;
		private IMongoCollection<TEntity> _collection;
		public void Init()
		{
			_store = Mongo.Create("mongodb://localhost/test");
			_collection = _store.GetCollection<TEntity>();
		}

		public void Save(TEntity e)
		{
			_collection.Save(e);
		}

		TEntity IBenchmark<TEntity>.Get(string id)
		{
			return _collection.FindOne(new { Id = id });

		}	


		public void Cleanup()
		{
			_store.Dispose();
		}


	}

}
