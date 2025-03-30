using LINQPad;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.Playwright;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.RegularExpressions;

namespace ScrapeUtils;

public static class ScriptRunner
{
	public static void Run(IPage page, Action<object> Log)
	{
		var code = ExtractPlayCode();
		if (code == null)
		{
			Log("[ScriptRunner] Failed to extract code");
			return;
		}

		code = Consts.ScriptUsings + code;

		if (Consts.DumpFormattedScriptWhenRunning)
			Log(Util.SyntaxColorText(code, SyntaxLanguageStyle.CSharp));

		var syntaxTree = CSharpSyntaxTree.ParseText(code);
		var baseFolder = Path.GetDirectoryName(typeof(System.Runtime.GCSettings).Assembly.Location)!;
		string[] refPaths = [
			Path.Combine(baseFolder, "System.Runtime.dll"),
			Path.Combine(baseFolder, "netstandard.dll"),
			typeof(object).Assembly.Location,
			typeof(Console).Assembly.Location,
			typeof(Regex).Assembly.Location,
			typeof(IPage).Assembly.Location,
		];

		var compilation = CSharpCompilation.Create(
			Path.GetRandomFileName(),
			syntaxTrees: [syntaxTree],
			references: refPaths.SelectA(e => MetadataReference.CreateFromFile(e)),
			options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
		);

		using var ms = new MemoryStream();
		var result = compilation.Emit(ms);
		if (!result.Success)
		{
			Log("[ScriptRunner] Compilation failed!");
			var failures = result.Diagnostics.WhereA(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity is DiagnosticSeverity.Error);
			foreach (var diagnostic in failures)
				Log($"\t{diagnostic.Id}: {diagnostic.GetMessage()}");
		}
		else
		{
			Log("[ScriptRunner] Compilation successful!");
			ms.Seek(0, SeekOrigin.Begin);
			var assembly = AssemblyLoadContext.Default.LoadFromStream(ms);
			var type = assembly.GetType("PlayClass");
			if (type == null)
			{
				Log("[ScriptRunner] Failed to find the Type");
				return;
			}
			var meth = type.GetMethod("Run", BindingFlags.Public | BindingFlags.Static);
			if (meth == null)
			{
				Log("[ScriptRunner] Failed to find the Method");
				return;
			}
			object[] args = [
				page,
				Log,
			];

			meth.Invoke(null, args);

			Log("[ScriptRunner] Done");
		}
	}



	static string? ExtractPlayCode()
	{
		var lines = File.ReadAllLines(Util.CurrentQueryPath);
		var idxStart = lines.Index().FirstOrDefault(e => e.Item2 == "public static class PlayClass", (-1, "")).Item1;
		if (idxStart == -1) return null;
		var idxEnd = lines.Index().Where(e => e.Index > idxStart).FirstOrDefault(e => e.Item2 == "}", (-1, "")).Item1;
		if (idxEnd == -1) return null;
		return lines.Skip(idxStart).Take(idxEnd - idxStart + 1).JoinLines();
	}

	static string JoinLines(this IEnumerable<string> source) => string.Join(Environment.NewLine, source);
	static T[] WhereA<T>(this IEnumerable<T> source, Func<T, bool> fun) => source.Where(fun).ToArray();
	static U[] SelectA<T, U>(this IEnumerable<T> source, Func<T, U> fun) => source.Select(fun).ToArray();
}