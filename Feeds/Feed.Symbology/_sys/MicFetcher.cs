using ExcelDataReader;
using BaseUtils;
using System.Text;

namespace Feed.Symbology._sys;

// https://www.iso20022.org/market-identifier-codes

static class MicFetcher
{
	public static Mic[] Fetch()
	{
		using var stream = Query();
		var xss = ConvertToCsv(stream);
		return xss
			.Skip(1)
			.SelectA(e => new Mic(
				e[0],
				e[1],
				e[2] switch
				{
					"OPRT" => true,
					"SGMT" => false,
					_ => throw new ArgumentException(),
				},
				e[3],
				e[4],
				e[5],
				e[6],
				e[7],
				e[8],
				e[9],
				e[10],
				e[11] switch
				{
					"ACTIVE" => MicStatus.Active,
					"EXPIRED" => MicStatus.Expired,
					"UPDATED" => MicStatus.Updated,
					_ => throw new ArgumentException(),
				},
				e[12].d(),
				e[13].d(),
				e[14].dn(),
				e[15].dn(),
				e[16],
				0,
				""
			))
			.Kidify();
	}


	sealed record Lvl(int Depth, string FullName);

	static Mic[] Kidify(this Mic[] xs)
	{
		var map = xs.ToDictionary(e => e.Name);
		var kidMap = xs.GroupBy(e => e.OperatingMic).ToDictionary(e => e.Key, e => e.Where(f => f.Name != f.OperatingMic).SelectA(f => f.Name));
		var levelMap = new Dictionary<string, Lvl>();
		var ys = new List<Mic>();
		var dads = xs.WhereA(e => e.IsRoot);
		var stack = new Stack<string>();
		void Rec(Mic n)
		{
			ys.Add(n);
			stack.Push(n.Name);
			levelMap[n.Name] = new Lvl(stack.Count - 1, string.Join(" ", stack));
			var kids = kidMap.TryGetValue(n.Name, out var arr) switch
			{
				true => arr.SelectA(e => map[e]),
				false => [],
			};
			foreach (var kid in kids)
				Rec(kid);
			stack.Pop();
		}
		foreach (var dad in dads)
			Rec(dad);
		return ys
			.SelectA(e => e with
			{
				Utils_Level = levelMap[e.Name].Depth,
				Utils_FullName = levelMap[e.Name].FullName,
			});
	}


	static DateOnly d(this string s) => DateOnly.ParseExact(s, "yyyyMMdd", null);
	static DateOnly? dn(this string s) => s == "" ? null : DateOnly.ParseExact(s, "yyyyMMdd", null);


	const string Url = "https://www.iso20022.org/sites/default/files/ISO10383_MIC/ISO10383_MIC.xlsx";

	static readonly HttpClient client = new();

	static Stream Query()
	{
		var response = client.GetAsync(Url).Result;
		if (!response.IsSuccessStatusCode) throw new ArgumentException($"Error querying. {response.StatusCode}");
		return response.Content.ReadAsStream();
	}

	static string[][] ConvertToCsv(Stream stream)
	{
		var list = new List<string[]>();

		Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
		using var reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
		var ds = reader.AsDataSet(new ExcelDataSetConfiguration()
		{
			ConfigureDataTable = _ => new ExcelDataTableConfiguration()
			{
				UseHeaderRow = false,
			},
		});

		int row_no = 0;
		while (row_no < ds.Tables[0].Rows.Count)
		{
			var xs = new string[ds.Tables[0].Columns.Count];
			for (var i = 0; i < ds.Tables[0].Columns.Count; i++)
				xs[i] = ds.Tables[0].Rows[row_no][i].ToString()!;
			row_no++;
			list.Add(xs);
		}

		return [..list];
	}
}