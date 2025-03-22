using System.Collections.Concurrent;
using System.Diagnostics;

namespace BacktraderLib;

public sealed record DispStats(
	int Created,
	int Disposed
);

public static class DispDiag
{
	// **********
	// * Public *
	// **********

	public static void Log(this Disp d)
	{
		L($"[Disp({d.Name})].Created");
		Disposable.Create(() => L($"[Disp({d.Name})].Disposed")).D(d);
	}


	public const string? BreakpointOnDispName = null;
	public static bool DisposeInReverseOrder { get; set; } = true;
	public static bool DispMakerLoggingEnabled { get; set; }

	public static DispStats GetStats() => new(countCreated, countDisposed);

	public static bool CheckForUndisposedDisps(bool waitForKey = true)
	{
		var isIssue = LogAndTellIfThereAreUndisposedDisps_Inner();
		if (isIssue && waitForKey)
			Console.ReadKey();
		return isIssue;
	}

	public static void ResetDispsForTests()
	{
		indentLevel = 0;
		map.Clear();
		countMap.Clear();
		countCreated = 0;
		countDisposed = 0;
	}


	// ************
	// * Internal *
	// ************
	internal static string NotifyNewDisp(Disp d, string nameBase, string srcFile, int srcLine)
	{
		countCreated++;
		var name = GetFullName(nameBase);
		if (name == BreakpointOnDispName)
			Debugger.Break();
		Print(name!, true, indentLevel++);
		map[d] = new DispNfo(d, name!, srcFile, srcLine);
		Disposable.Create(() =>
		{
			countDisposed++;
			Print(name!, false, --indentLevel);
			map[d] = map[d].FlagDispose();
		}).D(d);
		return name!;
	}


	// ***********
	// * Private *
	// ***********
	const int IndentSize = 2;
	const int ColName = 0x8537b0;
	const int ColNew = 0x3fe861;
	const int ColDispose = 0xeb3f76;

	static int indentLevel;
	static readonly ConcurrentDictionary<Disp, DispNfo> map = new();
	static readonly Dictionary<string, int> countMap = new();
	static int countCreated;
	static int countDisposed;


	static void Print(string name, bool isNew, int indent)
	{
		if (!DispMakerLoggingEnabled) return;
		var indentStr = new string(' ', indent * IndentSize);
		Console.Write(indentStr);
		Console.Write(name, ColName);
		var (txt, col) = isNew ? (".new()", ColNew) : (".Dispose()", ColDispose);
		Console.WriteLine(txt, col);
	}


	static bool LogAndTellIfThereAreUndisposedDisps_Inner()
	{
		var allDisps = map.Values.Where(e => !e.Disposed).ToArray();

		void LogCounts()
		{
			L("");
			L($"  # Disps created : {countCreated}");
			L($"  # Disps disposed: {countDisposed}");
			L("");
		}
		if (allDisps.Length == 0)
		{
			L("");
			LTitle("All Disps released");
			LogCounts();
			return false;
		}
		else
		{
			var topDisps = allDisps.RemoveSubs();
			LTitle($"{topDisps.Length} unreleased top level Disps (total: {allDisps.Length})");
			var pad = topDisps.Max(e => e.Name.Length);
			foreach (var d in topDisps)
				L($"    {d.Fmt(pad)}");
			LogCounts();
			return true;
		}
	}


	sealed record DispNfo(Disp Disp, string Name, string File, int Line)
	{
		public bool Disposed { get; private init; }

		public string Fmt(int pad) => $"{Name.PadRight(pad)} @ {Path.GetFileName(File)}:{Line}";
		public override string ToString() => $"{Name} @ {Path.GetFileName(File)}:{Line}";

		public DispNfo FlagDispose() => Disposed switch
		{
			true => throw new ArgumentException("Already disposed"),
			false => this with { Disposed = true }
		};
	}


	static void L(string s) => Console.WriteLine(s);

	static void LTitle(string s)
	{
		L(s);
		L(new string('=', s.Length));
	}

	static DispNfo[] RemoveSubs(this DispNfo[] ds) =>
		ds
			.Where(d => ds.Where(e => e != d).All(e => !e.Disp.Contains(d.Disp)))
			.ToArray();


	static string GetFullName(string name)
	{
		if (!countMap.TryAdd(name, 0))
			countMap[name]++;
		var cnt = countMap[name];
		return cnt switch
		{
			0 => name,
			_ => $"{name}[{cnt}]",
		};
	}
}