using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NoSqlBenchmarking
{
    class RavenDbBenchmark : IBenchmark
    {



        public void Init()
        {
            // none
        }

        public void Save(Dummy dummy)
        {
            throw new NotImplementedException();
        }

        public Dummy Get(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Cleanup()
        {
            // none
        }
    }
}
