using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NoSqlBenchmarking
{
	class SqlServerBenchmark : IBenchmark
	{

		private SqlConnection _connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Benchmark"].ConnectionString);

		public void Init()
		{
			_connection.Open();
		}

		public void Save(Dummy dummy)
		{
			var command = _connection.CreateCommand();
			command.CommandType = CommandType.StoredProcedure;
			command.CommandText = "InsertDummy";
			command.Parameters.AddWithValue("@_Id", dummy.Id);
			command.Parameters.AddWithValue("@Blob", dummy.Blob);
			command.ExecuteNonQuery();
		}

		public Dummy Get(string id)
		{
			var command = _connection.CreateCommand();
			command.CommandType = CommandType.StoredProcedure;
			command.CommandText = "GetDummy";
			command.Parameters.AddWithValue("@_Id", id);
			var reader = command.ExecuteReader( CommandBehavior.SingleRow);
			if(reader.Read())
			{
				var d = new Dummy()
				       	{
				       		Id = id,
							Blob = (byte[]) reader[1]
				       	};
				reader.Close();
				return d;
			}
			else
			{
				reader.Close();
				return null;
			}
		}

		public void Cleanup()
		{
			_connection.Close();
		}
	}
}
