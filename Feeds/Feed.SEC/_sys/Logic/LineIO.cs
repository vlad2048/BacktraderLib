using System.Text;
using BaseUtils;

namespace Feed.SEC._sys.Logic;

enum LineMethod
{
	OriginalEscaping,
	ReplaceTabChar,
}

static class LineIO
{
	const char TabChar = '\t';
	const char TabReplaceChar = '¬';
	const char QuoteChar = '"';



	public static string[] Line2Fields(string line, int? ensureColumnCount, LineMethod method) =>
		method switch
		{
			LineMethod.OriginalEscaping => Line2Fields_OriginalEscaping(line, ensureColumnCount),
			LineMethod.ReplaceTabChar => Line2Fields_ReplaceTabChar(line, ensureColumnCount),
			_ => throw new ArgumentException("Unknown LineMethod"),
		};


	public static string Fields2Line(string?[] fields, LineMethod method) =>
		method switch
		{
			LineMethod.OriginalEscaping => Fields2Line_OriginalEscaping(fields),
			LineMethod.ReplaceTabChar => Fields2Line_ReplaceTabChar(fields),
			_ => throw new ArgumentException("Unknown LineMethod"),
		};





	static string Fields2Line_ReplaceTabChar(string?[] fields) =>
		string.Join(TabChar, fields.Select(e => (e ?? string.Empty).Replace(TabChar, TabReplaceChar)));

	static string Fields2Line_OriginalEscaping(string?[] fields) =>
		string.Join(TabChar, fields.SelectA(ApplyOriginalEscaping));

	static string ApplyOriginalEscaping(this string? field)
	{
		if (field == null)
			return string.Empty;

		var hasTab = field.Contains('\t');
		var hasQuote = field.Contains('"');

		if (hasQuote)
			field = field.Replace("\"", "\"\"");
		if (hasTab || hasQuote)
			field = $"\"{field}\"";

		return field;
	}


	static string[] Line2Fields_ReplaceTabChar(string line, int? ensureColumnCount)
	{
		var xs = line.Split(TabChar).SelectA(e => e.Replace(TabReplaceChar, TabChar));
		if (ensureColumnCount.HasValue && xs.Length != ensureColumnCount.Value)
			throw new ArgumentException($"Expected {ensureColumnCount.Value} columns, but found {xs.Length}");
		return xs;
	}

	static string[] Line2Fields_OriginalEscaping(string line, int? ensureColumnCount) =>
		ensureColumnCount switch
		{
			not null => Line2Fields_OriginalEscaping_WithColumnCount(line, ensureColumnCount.Value),
			null => Line2Fields_OriginalEscaping_WithoutColumnCount(line),
		};

	static readonly StringBuilder sb = new(256);

	static string[] Line2Fields_OriginalEscaping_WithColumnCount(string line, int columnCount)
	{
		var results = new string[columnCount];
		var resultIndex = 0;
		//var sb = new StringBuilder(256);

		var inQuote = false;
		var onStart = true;


		void Add(char ch)
		{
			//$"{Pad}{sb}+[{ch.Fmt()}]".Log();
			sb.Append(ch);
		}
		void Flush()
		{
			var item = sb.ToString();
			//$"{Pad}ADD -> '{item}'".Log();
			results[resultIndex++] = item;
			//list.Add(item);
			sb.Clear();
			onStart = true;
		}


		for (var i = 0; i < line.Length; i++)
		{
			var ch = line[i];

			(int, bool) GetQuoteInfo()
			{
				var cnt = 0;
				while (i + cnt < line.Length && line[i + cnt] == '"') cnt++;
				i += Math.Max(0, cnt - 1);
				return (cnt, i == line.Length - 1);
			}
			var (quoteCount, quoteTerminate) = GetQuoteInfo();
			//Prn(ch, quoteCount, quoteTerminate);



			if (onStart)
			{
				if (inQuote) throw new ArgumentException("Impossible");
				if (quoteCount > 0)
				{
					inQuote = true;
					onStart = false;
					quoteCount--;
					if (quoteCount % 2 == 1) throw new ArgumentException("Impossible");
					if (quoteCount > 0) sb.Append(new string(QuoteChar, quoteCount / 2));
				}
				else if (ch == TabChar)
				{
					Flush();
				}
				else
				{
					onStart = false;
					Add(ch);
				}
			}
			else
			{
				if (!inQuote)
				{
					if (quoteCount > 0) throw new ArgumentException("Impossible");
					if (ch == TabChar)
					{
						Flush();
					}
					else
					{
						Add(ch);
					}
				}
				else
				{
					if (quoteCount > 0)
					{
						if (quoteCount % 2 == 0)
						{
							sb.Append(new string(QuoteChar, quoteCount / 2));
						}
						else
						{
							inQuote = false;
							if (quoteCount >= 3) sb.Append(new string(QuoteChar, (quoteCount - 1) / 2));
							var isEnd = i == line.Length - 1;
							var isTab = i < line.Length - 1 && line[i + 1] == TabChar;
							var isEndOrTab = isEnd || isTab;
							if (!isEndOrTab) throw new ArgumentException($"Impossible i={i}  str.Length={line.Length}");
						}
					}
					else
					{
						Add(ch);
					}
				}
			}


		}


		Flush();

		if (columnCount != results.Length)
			throw new ArgumentException($"Expected {columnCount} columns, but found {results.Length}");

		return results;
	}



	static string[] Line2Fields_OriginalEscaping_WithoutColumnCount(string line)
	{
		var results = new List<string>(36);

		var inQuote = false;
		var onStart = true;


		void Add(char ch)
		{
			//$"{Pad}{sb}+[{ch.Fmt()}]".Log();
			sb.Append(ch);
		}
		void Flush()
		{
			var item = sb.ToString();
			//$"{Pad}ADD -> '{item}'".Log();
			results.Add(item);
			sb.Clear();
			onStart = true;
		}


		for (var i = 0; i < line.Length; i++)
		{
			var ch = line[i];

			(int, bool) GetQuoteInfo()
			{
				var cnt = 0;
				while (i + cnt < line.Length && line[i + cnt] == '"') cnt++;
				i += Math.Max(0, cnt - 1);
				return (cnt, i == line.Length - 1);
			}
			var (quoteCount, quoteTerminate) = GetQuoteInfo();
			//Prn(ch, quoteCount, quoteTerminate);



			if (onStart)
			{
				if (inQuote) throw new ArgumentException("Impossible");
				if (quoteCount > 0)
				{
					inQuote = true;
					onStart = false;
					quoteCount--;
					if (quoteCount % 2 == 1) throw new ArgumentException("Impossible");
					if (quoteCount > 0) sb.Append(new string(QuoteChar, quoteCount / 2));
				}
				else if (ch == TabChar)
				{
					Flush();
				}
				else
				{
					onStart = false;
					Add(ch);
				}
			}
			else
			{
				if (!inQuote)
				{
					if (quoteCount > 0) throw new ArgumentException("Impossible");
					if (ch == TabChar)
					{
						Flush();
					}
					else
					{
						Add(ch);
					}
				}
				else
				{
					if (quoteCount > 0)
					{
						if (quoteCount % 2 == 0)
						{
							sb.Append(new string(QuoteChar, quoteCount / 2));
						}
						else
						{
							inQuote = false;
							if (quoteCount >= 3) sb.Append(new string(QuoteChar, (quoteCount - 1) / 2));
							var isEnd = i == line.Length - 1;
							var isTab = i < line.Length - 1 && line[i + 1] == TabChar;
							var isEndOrTab = isEnd || isTab;
							if (!isEndOrTab) throw new ArgumentException($"Impossible i={i}  str.Length={line.Length}");
						}
					}
					else
					{
						Add(ch);
					}
				}
			}


		}


		Flush();

		return [..results];
	}

}