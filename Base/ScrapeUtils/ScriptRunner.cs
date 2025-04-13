using System.Collections.Concurrent;
using System.Collections.Specialized;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.Playwright;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using BaseUtils;
using HtmlAgilityPack;
using System.Text.Encodings.Web;
using System.Web;

namespace ScrapeUtils;

public static class ScriptRunner
{
	public static async Task Run(string projFolder, IPage page, Action<object> log)
	{
		var codeFiles = Directory.GetFiles(projFolder, "*.cs", SearchOption.AllDirectories);
		var ass = Compile(codeFiles, log);
		if (ass == null) return;

		var meth = ExtractRunMethod(ass, log);
		if (meth == null) return;

		try
		{
			var tracing = Web.Tracing;
			object[] args = [page, tracing, log];
			var retObj = meth.Invoke(null, args);
			if (retObj == null)
			{
				log("[ScriptRunner] The script returned null");
				return;
			}

			var task = (Task)retObj;
			await task;
		}
		catch (Exception ex)
		{
			log(ex);
		}
	}


	static Assembly? Compile(string[] codeFiles, Action<object> log)
	{
		var baseFolder = Path.GetDirectoryName(typeof(System.Runtime.GCSettings).Assembly.Location)!;
		string[] refPaths = [
			Path.Combine(baseFolder, "System.Runtime.dll"),
			Path.Combine(baseFolder, "netstandard.dll"),
			typeof(object).Assembly.Location,
			typeof(Console).Assembly.Location,
			typeof(Enumerable).Assembly.Location,
			typeof(SortedDictionary<,>).Assembly.Location,
			typeof(ConcurrentDictionary<,>).Assembly.Location,
			typeof(Regex).Assembly.Location,
			typeof(JsonNode).Assembly.Location,
			typeof(IPage).Assembly.Location,
			typeof(Quarter).Assembly.Location,
			typeof(TNod<>).Assembly.Location,
			typeof(HtmlDocument).Assembly.Location,
			typeof(JavaScriptEncoder).Assembly.Location,
			typeof(HttpUtility).Assembly.Location,
			typeof(NameValueCollection).Assembly.Location,
		];
		var compilation = CSharpCompilation.Create(
			Path.GetRandomFileName(),
			syntaxTrees: codeFiles.Select(codeFile => CSharpSyntaxTree.ParseText(File.ReadAllText(codeFile))),
			references: refPaths.Select(e => MetadataReference.CreateFromFile(e)),
			options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
		);
		using var ms = new MemoryStream();
		var result = compilation.Emit(ms);
		if (!result.Success)
		{
			log("[ScriptRunner] Compilation failed!");
			var failures = result.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity is DiagnosticSeverity.Error);
			foreach (var diagnostic in failures)
				log($"\t{diagnostic.Id}: {diagnostic.GetMessage()}");
			//log(Util.SyntaxColorText(code, SyntaxLanguageStyle.CSharp));
			return null;
		}

		ms.Seek(0, SeekOrigin.Begin);
		return AssemblyLoadContext.Default.LoadFromStream(ms);
	}





	static MethodInfo? ExtractRunMethod(Assembly ass, Action<object> log)
	{
		var type = ass.GetType(Consts.ScriptClassName);

		if (type == null)
		{
			log($"[ScriptRunner] Cannot find class: {Consts.ScriptClassName}");
			return null;
		}

		var meth = type.GetMethod(Consts.ScriptMethodName, BindingFlags.Public | BindingFlags.Static);
		if (meth == null)
		{
			log($"[ScriptRunner] Cannot find public static method: {Consts.ScriptMethodName}");
			return null;
		}

		var methParams = meth.GetParameters();
		if (methParams.Length != 3)
		{
			log($"[ScriptRunner] Method needs to have 3 parameters but it has {methParams.Length}");
			return null;
		}

		if (methParams[0].ParameterType != typeof(IPage))
		{
			log($"[ScriptRunner] The first parameter needs to have type IPage but it has {methParams[0].ParameterType.Name}");
			return null;
		}

		if (methParams[1].ParameterType != typeof(ITracing))
		{
			log($"[ScriptRunner] The second parameter needs to have type ITracing but it has {methParams[1].ParameterType.Name}");
			return null;
		}

		if (methParams[2].ParameterType != typeof(Action<object>))
		{
			log($"[ScriptRunner] The third parameter needs to have type Action<object> but it has {methParams[2].ParameterType.Name}");
			return null;
		}

		if (meth.ReturnType != typeof(Task))
		{
			log($"[ScriptRunner] The method needs to return a Task but instead returns a {meth.ReturnType.Name}");
			return null;
		}

		return meth;
	}
}