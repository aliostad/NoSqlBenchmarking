using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NoSqlBenchmarking
{
    public interface IBenchmarkDummy
    {
        void Init();
        void Save(Dummy dummy);
        Dummy Get(string id);
        void Cleanup();
    }

	interface IBenchmark<TEntity> 
		where TEntity : class, IEntity 
	{
		void Init();
		void Save(TEntity e);
		TEntity Get(string id);
		void Cleanup();
	}
}
