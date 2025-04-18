using BaseUtils;
using HtmlAgilityPack;
using LINQPad;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.Playwright;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.Encodings.Web;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Web;
using RxLib;

namespace ScrapeUtils._sys;

static class ScriptRunner
{
	const string namespaceImports =
		"""
		global using System;
		global using System.Collections;
		global using System.Collections.Generic;
		global using System.Data;
		global using System.Diagnostics;
		global using System.IO;
		global using System.Linq;
		global using System.Linq.Expressions;
		global using System.Reflection;
		global using System.Text;
		global using System.Text.RegularExpressions;
		global using System.Threading;
		global using System.Transactions;
		global using System.Xml;
		global using System.Xml.Linq;
		global using System.Xml.XPath;
		
		global using System.Net.Http;
		global using System.Threading.Tasks;
		
		global using Microsoft.Playwright;
		
		global using BaseUtils;
		global using RxLib;
		global using System.Reactive.Linq;
		
		global using LINQPad;
		
		global using ScrapeUtils;
		
		
		""";

	static readonly string baseFolder = Path.GetDirectoryName(typeof(System.Runtime.GCSettings).Assembly.Location)!;

	static readonly string[] referencedAssemblyLocations =
	[
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
		typeof(DumpContainer).Assembly.Location,

		typeof(IRoVar<>).Assembly.Location,
		typeof(Obs).Assembly.Location,
		typeof(Util).Assembly.Location,

		typeof(ScriptRunner).Assembly.Location,
	];

	const string expectedClassName = "Script";
	const string expectedMethodName = "Run";
	static readonly Type[] expectedRunMethodArgumentTypes =
	[
		typeof(Web),
	];
	static readonly Type expectedMethodReturnType = typeof(Task);


	public static async Task Run(string[] codeUnits, Web web)
	{
		var ass = Compile(codeUnits, web.Log);
		if (ass == null) return;

		var meth = ExtractRunMethod(ass, web.Log);
		if (meth == null) return;

		try
		{
			await web.InitIFN();
			object[] args = [web];
			var retObj = meth.Invoke(null, args);
			if (retObj == null)
			{
				web.Log.Log("[ScriptRunner] The script returned null");
				return;
			}

			var task = (Task)retObj;
			await task;
		}
		catch (Exception ex)
		{
			web.Log.Log($"Unexpected error: [{ex.GetType().Name}] {ex.Message}");
		}
	}


	static Assembly? Compile(string[] codeUnits, DumpContainer logDC)
	{
		codeUnits = codeUnits.SelectA((e, i) => i switch
		{
			0 => namespaceImports + e,
			_ => e,
		});

		var compilation = CSharpCompilation.Create(
			Path.GetRandomFileName(),
			syntaxTrees: codeUnits.Select(codeUnit => CSharpSyntaxTree.ParseText(codeUnit)),
			references: referencedAssemblyLocations.Select(e => MetadataReference.CreateFromFile(e)),
			options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
		);
		using var ms = new MemoryStream();
		var result = compilation.Emit(ms);
		if (!result.Success)
		{
			logDC.Log("Compilation failed!");
			var failures = result.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity is DiagnosticSeverity.Error);
			foreach (var diagnostic in failures)
				logDC.Log($"\t{diagnostic.Id}: {diagnostic.GetMessage()}");
			foreach (var codeUnit in codeUnits)
				logDC.AppendContent(Util.SyntaxColorText(codeUnit, SyntaxLanguageStyle.CSharp));
			return null;
		}

		ms.Seek(0, SeekOrigin.Begin);
		return AssemblyLoadContext.Default.LoadFromStream(ms);
	}



	static MethodInfo? ExtractRunMethod(Assembly ass, DumpContainer logDC) =>
		(
			from type in ass.GetTypes()
			where type.Name == expectedClassName
			from meth in type.GetMethods(BindingFlags.Public | BindingFlags.Static)
			where
				meth.Name == expectedMethodName &&
				meth.ReturnType == expectedMethodReturnType &&
				meth.GetParameters().Length == expectedRunMethodArgumentTypes.Length &&
				meth.GetParameters().Zip(expectedRunMethodArgumentTypes).All(t => t.First.ParameterType == t.Second)
			select meth
		)
		.SingleToLog(logDC);



	static T? SingleToLog<T>(this IEnumerable<T> source, DumpContainer logDC) where T : class
	{
		var arr = source.ToArray();
		switch (arr.Length)
		{
			case 1:
				return arr[0];
			case 0:
				logDC.Log($"Failed to find the {expectedMethodName}() method");
				return null;
			case > 1:
				logDC.Log($"Found {arr.Length} ambiguous {expectedMethodName}() methods");
				return null;
			default:
				logDC.Log("Impossible");
				return null;
		}
	}
	


	static void Log(this DumpContainer logDC, string str) => logDC.AppendContent($"[ScriptRunner - Error] {str}");
}