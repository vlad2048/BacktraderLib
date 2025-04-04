/*
using Microsoft.Playwright;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;

namespace ScrapeUtils;

public static class ScriptRunnerAssembly
{
	public static async Task Run(string assFile, IPage page, Action<object> log)
	{
		await Call(assFile, page, log);
	}


	static async Task Call(string assemblyPath, IPage page, Action<object> log)
	{
		var weakRef = await ExecuteAndUnload(assemblyPath, page, log);
		for (var i = 0; weakRef.IsAlive && i < 10; i++)
		{
			if (i > 0)
			{
				await Task.Delay(TimeSpan.FromSeconds(1));
			}
			log($"Collect {i + 1}/10");
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}

		if (weakRef.IsAlive)
		{
			log("Collect failed");
		}
		else
		{
			log("Collect success");
		}
	}


	[MethodImpl(MethodImplOptions.NoInlining)]
	static async Task<WeakReference> ExecuteAndUnload(string assemblyPath, IPage page, Action<object> log)
	{
		var ctx = new UnloadableAssemblyLoadContext(assemblyPath);
		var ass = ctx.LoadFromAssemblyPath(assemblyPath);

		var weakRef = new WeakReference(ctx, trackResurrection: true);

		var meth = ExtractRunMethod(ass, log);
		if (meth == null) return weakRef;

		try
		{
			object[] args = [page, log];
			var retObj = meth.Invoke(null, args);
			if (retObj == null)
			{
				log("[ScriptRunner] The script returned null");
				return weakRef;
			}

			var task = (Task)retObj;
			await task;
		}
		catch (Exception ex)
		{
			log(ex);
		}

		ctx.Unload();

		return weakRef;
	}




	sealed class UnloadableAssemblyLoadContext(string mainAssemblyToLoadPath) : AssemblyLoadContext(isCollectible: true)
	{
		readonly AssemblyDependencyResolver _resolver = new(mainAssemblyToLoadPath);

		protected override Assembly? Load(AssemblyName name)
		{
			var assemblyPath = _resolver.ResolveAssemblyToPath(name);
			return assemblyPath switch
			{
				not null => LoadFromAssemblyPath(assemblyPath),
				null => null,
			};
		}
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
		if (methParams.Length != 2)
		{
			log($"[ScriptRunner] Method needs to have 2 parameters but it has {methParams.Length}");
			return null;
		}

		if (methParams[0].ParameterType != typeof(IPage))
		{
			log($"[ScriptRunner] The first parameter needs to have type IPage but it has {methParams[0].ParameterType.Name}");
			return null;
		}

		if (methParams[1].ParameterType != typeof(Action<object>))
		{
			log($"[ScriptRunner] The second parameter needs to have type Action<object> but it has {methParams[1].ParameterType.Name}");
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
*/