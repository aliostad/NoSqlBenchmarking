using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NoSqlBenchmarking.Imdb
{
	class Film
	{
		public Film()
		{
			
		}

		public Film(string name, int year)
		{
			Name = name.Trim('"');
			Year = year;
		}

		public string Name { get; set; }
		public int Year { get; set; }

		public override int GetHashCode()
		{
			return (Name + Year).GetHashCode();
		}

		public override bool Equals(object obj)
		{
			var film = obj as Film;
			if (film == null)
				return false;
			return this.GetHashCode() == film.GetHashCode();
		}
	}
}
