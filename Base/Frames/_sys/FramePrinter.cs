using System.Drawing;
using System.Text;
using BaseUtils;
using Frames._sys.Utils;
using PrettyPrinting;

namespace Frames._sys;

static class FramePrinter
{
	const int Padding = 1;
	const int MaxDecimalDigits = 6;
	const string DateFmt = "{0:yyyy-MM-dd}";
	const int DateWidth = 10 + 2 * Padding;
	const string EllipseStr = "...";

	static readonly Color Fore = Color.FromArgb(0xBCBCBC);
	static readonly Color BackCol0 = Color.FromArgb(0x272727);
	static readonly Color BackCol1 = Color.FromArgb(0x2D2D2D);
	static readonly Color BackRow0 = Color.FromArgb(0x151515);
	static readonly Color BackRow1 = Color.FromArgb(0x1D1D1D);


	public static int RowMax { get; set; } = 60;
	public static int RowSmp { get; set; } = 5;
	public static int ColMax { get; set; } = 10;
	public static int ColSmp { get; set; } = 5;




	// ***************
	// ***************
	// **** Serie ****
	// ***************
	// ***************
	public static TxtArray Print<N>(Serie<N> serie)
	{
		var fmt = GetSerieFmt(serie);
		return Txt.Build(t =>
		{
			t.SetFore(Fore);

			t.SetBack(BackCol0);
			t.Write(Pad(DateWidth));
			t.Write($"{serie.Name}".Fmt(fmt.Width, true));
			t.WriteLine();

			Sample(
				SplitRangeDefault(serie.RowCount, RowMax, RowSmp),
				(i, nfo) =>
				{
					t.SetBack((i - nfo.Ofs) % 2 == 0 ? BackRow0 : BackRow1);
					t.Write(serie.Index[i].Fmt());
					t.Write(serie.Values[i].Fmt(fmt));
					t.WriteLine();
				},
				() =>
				{
					t.SetBack(BackRow1);
					t.Write(FmtEllipse(DateWidth));
					t.Write(FmtEllipse(fmt.Width));
					t.WriteLine();
				}
			);
		});
	}





	// **********************
	// **********************
	// **** Frame<N, K1> ****
	// **********************
	// **********************
	public static TxtArray Print<N, K1>(Frame<N, K1> frame)
	{
		var cols = frame.ToArray();
		var fmts = cols.SelectA(GetSerieFmt);

		return Txt.Build(t =>
		{
			t.SetFore(Fore);

			t.SetBack(BackCol0);
			var splitRange = SplitRangeDefault(frame.ColCount, ColMax, ColSmp);
			t.Write(splitRange.Item2 == null ? Pad(DateWidth) : $"x{frame.ColCount}".Fmt(DateWidth));
			Sample(
				splitRange,
				(j, _) =>
				{
					t.Write($"{cols[j].Name}".Fmt(fmts[j].Width, true));
				},
				() =>
				{
					t.Write(FmtEllipse());
				}
			);
			t.WriteLine();

			Sample(
				SplitRangeDefault(frame.RowCount, RowMax, RowSmp),
				(i, nfo) =>
				{
					t.SetBack((i - nfo.Ofs) % 2 == 0 ? BackRow0 : BackRow1);
					t.Write(frame.Index[i].Fmt());
					Sample(
						SplitRangeDefault(frame.ColCount, ColMax, ColSmp),
						(j, _) =>
						{
							t.Write(cols[j].Values[i].Fmt(fmts[j]));
						},
						() =>
						{
							t.Write(FmtEllipse());
						}
					);
					t.WriteLine();
				},
				() =>
				{
					t.SetBack(BackRow1);
					t.Write(FmtEllipse(DateWidth));
					Sample(
						SplitRangeDefault(frame.ColCount, ColMax, ColSmp),
						(j, _) =>
						{
							t.Write(FmtEllipse(fmts[j].Width));
						},
						() =>
						{
							t.Write(FmtEllipse());
						}
					);
					t.WriteLine();
				}
			);
		});
	}





	// **************************
	// **************************
	// **** Frame<N, K1, K2> ****
	// **************************
	// **************************
	public static TxtArray Print<N, K1, K2>(Frame<N, K1, K2> frame)
	{
		var (cols, colSplitCnt, isColSampleConnected) = frame.GetCols();
		var colsFmt = cols.SelectA(GetTopLevelFmt);

		return Txt.Build(t =>
		{
			t.SetFore(Fore);

			t.SetBack(BackCol0);
			var splitRange = SplitRangeAt(cols.Length, colSplitCnt);
			t.Write(splitRange.Item2 == null ? Pad(DateWidth) : $"x{frame.ColCount}".Fmt(DateWidth));
			Sample(
				splitRange,
				(j, nfo) =>
				{
					var col1Name = cols[j].Item1;
					var col1NameStr = $"{col1Name}";
					if (nfo.IsSecond && j - nfo.Ofs == 0 && isColSampleConnected)
						col1NameStr = string.Empty;
					t.Write(col1NameStr.Fmt(colsFmt[j].TotalWidth));
				},
				() =>
				{
					var ellipseStr = EllipseStr;
					if (isColSampleConnected)
						ellipseStr = new string(' ', EllipseStr.Length);
					t.Write(ellipseStr.Fmt(ellipseStr.Length.AddPadding(), true));
				}
			);
			t.WriteLine();

			t.SetBack(BackCol1);
			t.Write(Pad(DateWidth));
			Sample(
				SplitRangeAt(cols.Length, colSplitCnt),
				(j, _) =>
				{
					var col = cols[j];
					var colFmt = colsFmt[j];
					for (var j2 = 0; j2 < col.Item2.Length; j2++)
					{
						var subCol = col.Item2[j2];
						var subColFmt = colFmt.Fmts[j2];
						t.Write($"{subCol.Name}".Fmt(subColFmt.Width, true));
					}
					t.Write(Pad(colFmt.LeftoverWidth));
				},
				() =>
				{
					t.Write(FmtEllipse());
				}
			);
			t.WriteLine();

			Sample(
				SplitRangeDefault(frame.RowCount, RowMax, RowSmp),
				(i, nfo) =>
				{
					t.SetBack((i - nfo.Ofs) % 2 == 0 ? BackRow0 : BackRow1);
					t.Write(frame.Index[i].Fmt());
					Sample(
						SplitRangeAt(cols.Length, colSplitCnt),
						(j, _) =>
						{
							var col = cols[j];
							var colFmt = colsFmt[j];
							for (var j2 = 0; j2 < col.Item2.Length; j2++)
							{
								var subCol = col.Item2[j2];
								var subColFmt = colFmt.Fmts[j2];
								t.Write(subCol.Values[i].Fmt(subColFmt));
							}
							t.Write(Pad(colFmt.LeftoverWidth));
						},
						() =>
						{
							t.Write(FmtEllipse());
						}
					);
					t.WriteLine();
				},
				() =>
				{
					t.SetBack(BackRow1);
					t.Write(FmtEllipse(DateWidth));
					Sample(
						SplitRangeAt(cols.Length, colSplitCnt),
						(j, _) =>
						{
							var col = cols[j];
							var colFmt = colsFmt[j];
							for (var j2 = 0; j2 < col.Item2.Length; j2++)
							{
								var subColFmt = colFmt.Fmts[j2];
								t.Write(FmtEllipse(subColFmt.Width));
							}
							t.Write(Pad(colFmt.LeftoverWidth));
						},
						() =>
						{
							t.Write(FmtEllipse());
						}
					);
					t.WriteLine();
				}
			);
		});
	}





	// GetCols & TopLevelFmt
	// =====================
	static ((K1, Serie<K2>[])[], int?, bool) GetCols<N, K1, K2>(this Frame<N, K1, K2> frame)
	{
		var all = frame.SelectA(e => (e.Name, Series: e.ToArray()));
		if (all.SelectMany(e => e.Series).Count() <= ColMax)
			return (all, null, false);

		var allIndices = all.SelectManyA((e1, i1) => e1.Series.Select((_, i2) => (i1, i2)));

		var (indicesPrev, indicesNext) = (
			allIndices.Take(ColSmp).ToArray(),
			allIndices.TakeLast(ColSmp).ToArray()
		);

		(K1, Serie<K2>[])[] Get((int, int)[] indices) =>
			indices.GroupBy(e => e.Item1)
				.SelectA(g => (
					all[g.Key].Name,
					g.SelectA(t => all[t.Item1].Series[t.Item2])
				));

		var splitPrev = indicesPrev.Last();
		var splitNext = indicesNext.First();
		var isColSampleConnected = splitPrev.i1 == splitNext.i1;

		return (Get(indicesPrev).Concat(Get(indicesNext)).ToArray(), indicesPrev.Last().i1 + 1, isColSampleConnected);
	}


	sealed record TopLevelFmt(int TotalWidth, int LeftoverWidth, SerieFmt[] Fmts);

	static TopLevelFmt GetTopLevelFmt<K1, K2>((K1, Serie<K2>[]) col)
	{
		var fmts = col.Item2.SelectA(GetSerieFmt);
		var widthName = $"{col.Item1}".Length.AddPadding();
		var widthCols = fmts.Sum(e => e.Width);
		return (widthCols >= widthName) switch
		{
			true => new TopLevelFmt(widthCols, 0, fmts),
			false => new TopLevelFmt(widthName, widthName - widthCols, fmts),
		};
	}



	// Sample
	// ======
	sealed record SplitRange(Range Prev, Range Next);

	static (int, SplitRange?) SplitRangeDefault(int n, int nMax, int nSmp) =>
	(
		n,
		(n <= nMax) switch
		{
			true => null,
			false => new SplitRange(
				Range.EndAt(new Index(nSmp)),
				Range.StartAt(new Index(nSmp, true))
			),
		}
	);

	static (int, SplitRange?) SplitRangeAt(int n, int? splitIdx) =>
	(
		n,
		// ReSharper disable once MergeIntoPattern
		// ReSharper disable once MergeSequentialChecks
		(splitIdx.HasValue && splitIdx.Value >= 1 && splitIdx.Value <= n - 1) switch
		{
			true => new SplitRange(
#pragma warning disable CS8629 // Nullable value type may be null.
				Range.EndAt(new Index(splitIdx.Value)),
#pragma warning restore CS8629 // Nullable value type may be null.
				Range.StartAt(new Index(splitIdx.Value))
			),
			false => null,
		}
	);

	sealed record SampleNfo(int Ofs, bool IsSecond);

	static void Sample(
		(int, SplitRange?) t,
		Action<int, SampleNfo> printRow,
		Action printEllipse
	)
	{
		var (n, splitRange) = t;

		void PrintRange(Range range, bool isSecond)
		{
			var (ofs, lng) = range.GetOffsetAndLength(n);
			for (var i = ofs; i < ofs + lng; i++)
				printRow(i, new SampleNfo(ofs, isSecond));
		}

		if (splitRange != null)
		{
			PrintRange(splitRange.Prev, false);
			printEllipse();
			PrintRange(splitRange.Next, true);
		}
		else
		{
			PrintRange(Range.All, false);
		}
	}



	// SerieFmt
	// ========
	sealed record SerieFmt(int Width, string Fmt)
	{
		public int WidthWithoutPadding => Width.SubPadding();
	}

	static SerieFmt GetSerieFmt<N>(Serie<N> serie)
	{
		static string MkFmt(int n) => $"{{0:F{n}}}";
		if (serie.Values.Length == 0) return new SerieFmt($"{serie.Name}".Length.AddPadding(), MkFmt(MaxDecimalDigits));

		static int GetDecimalDigits(double v)
		{
			var str = v.ToString(System.Globalization.CultureInfo.InvariantCulture);
			var idx = str.IndexOf('.');
			return idx switch
			{
				-1 => 0,
				_ => str.Length - idx - 1,
			};
		}

		static int GetLng(double v, string fmt) =>
			v.IsNaN() switch
			{
				false => string.Format(fmt, v).Length,
				true => 1,
			};

		var digits = Math.Min(MaxDecimalDigits, serie.Values.Max(GetDecimalDigits));
		var fmt = MkFmt(digits);

		var width = Max(
			$"{serie.Name}".Length,
			serie.Values.Max(e => GetLng(e, fmt)),
			serie.Values.Length > RowMax ? EllipseStr.Length : 0
		).AddPadding();

		return new SerieFmt(width, fmt);
	}



	// Fmt
	// ===
	static string FmtEllipse(int? width = null) => EllipseStr.Fmt(width ?? EllipseStr.Length.AddPadding(), true);

	static string Fmt(this DateTime t) => string.Format(DateFmt, t).Fmt(DateWidth);

	static string Fmt(this string s, int width, bool padLeft = false)
	{
		var sb = new StringBuilder();
		sb.Append(Pad(Padding));
		sb.Append(s.PadTrunc(padLeft, width.SubPadding()));
		sb.Append(Pad(Padding));
		var str = sb.ToString();
		if (str.Length != width) throw new ArgumentException("Impossible");
		return str;
	}

	static string Fmt(this double v, SerieFmt fmt)
	{
		var sb = new StringBuilder();
		sb.Append(Pad(Padding));
		sb.Append((v.IsNaN() ? "_" : string.Format(fmt.Fmt, v)).PadLeft(fmt.WidthWithoutPadding));
		sb.Append(Pad(Padding));
		var str = sb.ToString();
		if (str.Length != fmt.Width) throw new ArgumentException("Impossible");
		return str;
	}



	// String Utils
	// ============
	static string PadTrunc(this string s, bool padLeft, int n)
	{
		var res = padLeft ? s.PadLeft(n) : s.PadRight(n);
		if (res.Length > n) res = res[..n];
		return res;
	}
	static string Pad(int n) => new(' ', n);
	static int AddPadding(this int e) => e + 2 * Padding;
	static int SubPadding(this int e) => e - 2 * Padding;

	static int Max(params int[] xs) => xs.Max();
}
