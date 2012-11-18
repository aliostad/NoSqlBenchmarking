using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NoSqlBenchmarking.Imdb;

namespace NoSqlBenchmarking.Imdb
{
	class DirectorsListReader : IDisposable
	{
		private string _fileName;
		private StreamReader _reader;
		private static Regex _directorLinePattern = new Regex(@"^([^\t]+)\t+(.*?) \((\d{4})\)", RegexOptions.Compiled);
		private static Regex _filmLinePattern = new Regex(@"^\t+(.*?) \((\d{4})\)", RegexOptions.Compiled);

		public DirectorsListReader(string fileName)
		{
			_fileName = fileName;
			_reader = new StreamReader(fileName);
			ReadUntilTop();
		}

		private void ReadUntilTop()
		{
			while (true)
			{
				var line = _reader.ReadLine();
				if(line.StartsWith("----			------"))
					return;
				if(line == null)
					return;
			}
		}

		public bool Read(out Director director)
		{
			director = null;
			var directorLine = ReadDirectorLine(_reader);
			if (directorLine == null)
				return false;

			var match = _directorLinePattern.Match(directorLine);
			director = new Director(match.Groups[1].Value);
			director.Films.Add(new Film(match.Groups[2].Value, int.Parse(match.Groups[3].Value)));

			while (true)
			{
				var line = _reader.ReadLine();
				match = _filmLinePattern.Match(line);
				if (!match.Success)
					return true;

				director.Films.Add(new Film(match.Groups[1].Value, int.Parse(match.Groups[2].Value)));					
			}
		}

		private static string ReadDirectorLine(StreamReader reader)
		{
			while (true)
			{
				var line = reader.ReadLine();
				if (line == null)
					return null;
				if (_directorLinePattern.IsMatch(line))
					return line;
			}
		}


		public void Dispose()
		{
			_reader.Dispose();
		}
	}
}
