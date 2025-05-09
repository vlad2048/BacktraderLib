using BaseUtils;
using LINQPad;

namespace FeedUtils;

public interface ICacheStrat;
sealed record NeverCacheStrat : ICacheStrat;
sealed record AlwaysCacheStrat : ICacheStrat;
sealed record FilesUpdatedCacheStrat(string[] FilePatterns) : ICacheStrat;
sealed record TooOldCacheStrat(TimeSpan MaxAge) : ICacheStrat;

public static class CacheStrat
{
	public static readonly ICacheStrat Never = new NeverCacheStrat();
	public static readonly ICacheStrat Always = new AlwaysCacheStrat();
	public static ICacheStrat FilesUpdated(params string[] filePatterns) => new FilesUpdatedCacheStrat(filePatterns);
	public static ICacheStrat TooOld(TimeSpan maxAge) => new TooOldCacheStrat(maxAge);
}

public sealed class CacheFile<T>
{
	readonly string file;
	readonly ICacheStrat strat;
	readonly Func<T> fun;
	readonly Jsoner jsoner;
	readonly Lazy<T> lazy;

	public CacheFile(string file, ICacheStrat strat, Func<T> fun, Jsoner? jsoner = null)
	{
		(this.file, this.strat, this.fun, this.jsoner) = (file, strat, fun, jsoner ?? Jsoner.I);
		lazy = new Lazy<T>(Get);
	}


	public T Value => lazy.Value;


	T Get() =>
		NeedRefresh() switch
		{
			false => jsoner.Load<T>(file),
			true => jsoner.Save(FunWithLog(), file),
		};

	T FunWithLog()
	{
		$"Refresh '{file}'".Dump();
		return fun();
	}

	bool NeedRefresh() =>
		File.Exists(file) switch
		{
			false => true,
			true => strat switch
			{
				NeverCacheStrat => false,

				AlwaysCacheStrat => true,

				FilesUpdatedCacheStrat { FilePatterns: var filePatterns } => filePatterns.GetFilePatternsTime() > file.GetFileTime(),

				TooOldCacheStrat { MaxAge: var maxAge } => DateTime.Now - file.GetFileTime() > maxAge,

				_ => throw new ArgumentException("Unknown CacheStrat"),
			},
		};
}


file static class CacheFileUtils
{
	public static DateTime? GetFilePatternsTime(this string[] filePatterns)
	{
		var files = GetFilePatternsFiles(filePatterns);
		return files.Length switch
		{
			0 => null,
			_ => files.Max(e => e.GetFileTime()),
		};
	}

	public static DateTime GetFileTime(this string file) => new FileInfo(file).LastWriteTime;


	static string[] GetFilePatternsFiles(string[] filePatterns) => filePatterns.SelectManyA(GetFilePatternFiles);

	static string[] GetFilePatternFiles(string filePattern) =>
		filePattern.Contains('*') switch
		{
			false => [filePattern],
			true => Directory.GetFiles(Path.GetDirectoryName(filePattern)!, Path.GetFileName(filePattern)),
		};
}




/*{
	if (strat is NoneCacheStrat)
		return fun();
	else if (strat is FilesUpdatedCacheStrat filesUpdated)
	{
		var files = filesUpdated.FilePattern.Split(';').SelectMany(e => e.GetFiles()).ToArray();
		if (files.Length == 0)
			throw new Exception($"No files found for pattern '{filesUpdated.FilePattern}'");
		if (files.Any(e => e.LastWriteTime > DateTime.Now - TimeSpan.FromDays(1)))
			return fun();
	}
	else if (strat is TooOldCacheStrat tooOld)
	{
		if (DateTime.Now - DateTime.Now.Add(tooOld.MaxAge) > tooOld.MaxAge)
			return fun();
	}

	throw new Exception("Cache not updated");
}*/






/*
public interface ICacheFile
{
	bool RefreshIFN();
}

public sealed class CacheFile<T> : ICacheFile
{
	readonly string File;
	readonly Func<T> Fun;
	readonly ICacheFile[] Dependencies;
	readonly TimeSpan RefreshPeriod;
	readonly Jsoner Jsoner;
	readonly Lazy<T> lazy;

	public CacheFile(
		string file,
		Expression<Func<T>> expr,
		TimeSpan? refreshPeriod = null,
		Jsoner? jsoner = null
	)
	{
		File = file;
		Fun = expr.Compile();
		Dependencies = expr.FindAll<ICacheFile, T>();
		RefreshPeriod = refreshPeriod ?? TimeSpan.FromDays(1);
		Jsoner = jsoner ?? Jsoner.I;

		lazy = new Lazy<T>(() =>
		{
			RefreshIFN();
			return Jsoner.Load<T>(File);
		});
	}

	public T Value => lazy.Value;

	public bool RefreshIFN()
	{
		var force = Dependencies.SelectA(e => e.RefreshIFN()).Any(e => e);
		if (force || NeedsRefresh())
		{
			Refresh();
			return true;
		}
		return false;
	}

	bool NeedsRefresh() => !(System.IO.File.Exists(File) && DateTime.Now - new FileInfo(File).LastWriteTime <= RefreshPeriod);
	void Refresh()
	{
		$"Refresh '{File}'".Dump();
		Jsoner.Save(Fun(), File);
	}
}



file static class CacheFileExprUtils
{
	public static F[] FindAll<F, T>(this Expression<Func<T>> expr)
	{
		var visitor = new Picker<F>();
		visitor.Visit(expr);
		return visitor.Vals;
	}

	sealed class Picker<F> : ExpressionVisitor
	{
		readonly List<F> vals = [];

		public F[] Vals => [.. vals];

		public override Expression? Visit(Expression? expr)
		{
			if (expr != null)
			{
				if (expr.Is<MemberExpression>(ExpressionType.MemberAccess, out var exprMember) && exprMember.Member.Name == "Value")
				{
					var pickExpr = Expression.Lambda<Func<F>>(
						exprMember.Expression!,
						Array.Empty<ParameterExpression>()
					).Compile();
					var obj = pickExpr();
					vals.Add(obj);
				}
			}
			return base.Visit(expr);
		}
	}


	static bool Is<T>(this Expression expr, ExpressionType exprType, [NotNullWhen(true)] out T? typedExpr) where T : Expression
	{
		if (expr is T typedExpr_ && expr.NodeType == exprType)
		{
			typedExpr = typedExpr_;
			return true;
		}
		else
		{
			typedExpr = null;
			return false;
		}
	}
}
*/