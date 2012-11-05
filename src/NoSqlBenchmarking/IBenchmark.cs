using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NoSqlBenchmarking
{
    public interface IBenchmark
    {
        void Init();
        void Save(Dummy dummy);
        Dummy Get(Guid id);
        void Cleanup();
    }
}
