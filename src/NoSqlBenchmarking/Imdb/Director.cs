using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NoSqlBenchmarking.Imdb
{
	class Director
	{

		public Director()
		{
			Films = new List<Film>();
		}

		public Director(string name) : this()
		{
			var strings = name.Split(',');
			Surname = strings[0];
			Forename = strings.Length > 1 ? strings[1] : string.Empty;
			Name = name;
		}

		public string Name { get; set; }
		public string Forename { get; set; }
		public string Surname { get; set; }
		public List<Film> Films { get; set; }

		public override string ToString()
		{
			return Name;
		}
	}
}
