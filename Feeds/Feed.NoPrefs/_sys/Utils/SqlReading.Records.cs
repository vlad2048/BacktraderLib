using System.Data.Common;
using System.Reflection;
using BaseUtils;
using MySqlConnector;

namespace Feed.NoPrefs._sys.Utils;

sealed class SqlReadingException(string query, MySqlException innerEx) : Exception(innerEx.Message, innerEx)
{
	public string Query { get; } = query;
}

static partial class SqlReading
{
	public static T[] Read<T>(this MySqlConnection conn, string sql)
	{
		try
		{
			var list = new List<T>();
			using var cmd = new MySqlCommand(sql, conn);
			using var reader = cmd.ExecuteReader();
			var dbSchema = reader.GetDbSchema();
			var csSchema = GetCsSchema<T>();
			var args = MatchColumns(dbSchema, csSchema);

			if (reader.HasRows)
				while (reader.Read())
				{
					var argVals = reader.ReadArgs(args);
					var obj = (T)(Activator.CreateInstance(typeof(T), argVals) ?? throw new ArgumentException("Activator.CreateInstance returned null"));
					list.Add(obj);
				}

			return [.. list];
		}
		catch (MySqlException ex)
		{
			throw new SqlReadingException(sql, ex);
		}
	}



	// ******************
	// * Read Arguments *
	// ******************
	static object?[] ReadArgs(this MySqlDataReader reader, IEnumerable<Arg> args) => args.SelectA(arg => reader.Map(arg.DbCol, arg.CsCol.Type));



	// ***********
	// * Structs *
	// ***********
	sealed record Arg(DbCol DbCol, CsCol CsCol);

	sealed record DbCol(int Idx, string Name, Type CsType, string DbType, bool AllowNull)
	{
		public string NameNorm => Name.Norm();
		public static DbCol Make(DbColumn e) => new(
			e.ColumnOrdinal ?? throw new ArgumentException("ColumnOrdinal is null"),
			e.ColumnName,
			e.DataType ?? throw new ArgumentException("DataType is null"),
			e.DataTypeName ?? throw new ArgumentException("DataTypeName is null"),
			e.AllowDBNull ?? throw new ArgumentException("AllowDBNull is null")
		);
	}

	sealed record CsCol(string Name, Type Type)
	{
		public string NameNorm => Name.Norm();
		public static CsCol Make(PropertyInfo e) => new(
			e.Name,
			e.PropertyType
		);
	}



	// *******************
	// * Column Matching *
	// *******************
	static Arg[] MatchColumns(DbCol[] dbSchema, CsCol[] csSchema)
	{
		if (dbSchema.Length != csSchema.Length) throw new ArgumentException("Mismatched columns/properties lengths");
		var n = dbSchema.Length;
		var args = new Arg[n];
		var dbCols = dbSchema.ToList();
		for (var i = 0; i < n; i++)
		{
			var csCol = csSchema[i];
			var bestDbCol = FindBestDbCol(csCol, dbCols);
			dbCols.Remove(bestDbCol);
			args[i] = new Arg(bestDbCol, csCol);
		}
		return args;
	}

	static DbCol FindBestDbCol(CsCol csCol, List<DbCol> dbCols) => dbCols.MinBy(dbCol => StringDistance(dbCol.NameNorm, csCol.NameNorm)) ?? throw new ArgumentException("Shouldn't return null");



	// ******************
	// * Schema Readers *
	// ******************
	static DbCol[] GetDbSchema(this DbDataReader reader) => reader.GetColumnSchema().SelectA(DbCol.Make);
	static CsCol[] GetCsSchema<T>() =>
		IsRecord<T>() switch
		{
			true => typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public).SelectA(CsCol.Make),
			false => throw new NotImplementedException("Only records are supported for now"),
		};

	static bool IsRecord<T>() => typeof(T).GetMethods().Any(m => m.Name == "<Clone>$");



	// ****************
	// * String Utils *
	// ****************
	static string Norm(this string s) =>
		   s
			   .Replace("_", "")
			   .ToLowerInvariant();

	static int StringDistance(string a, string b) =>
		(string.IsNullOrEmpty(a), string.IsNullOrEmpty(b)) switch
		{
			(true, true) => 0,
			(true, false) => b.Length,
			(false, true) => a.Length,
			(false, false) => StringDistanceNotEmpty(a, b),
		};

	static int StringDistanceNotEmpty(string a, string b)
	{
		var lngA = a.Length;
		var lngB = b.Length;
		var distances = new int[lngA + 1, lngB + 1];

		// ReSharper disable EmptyEmbeddedStatement
		for (int i = 0; i <= lngA; distances[i, 0] = i++) ;
		for (int j = 0; j <= lngB; distances[0, j] = j++) ;
		// ReSharper restore EmptyEmbeddedStatement

		for (int i = 1; i <= lngA; i++)
			for (int j = 1; j <= lngB; j++)
			{
				var cost = b[j - 1] == a[i - 1] ? 0 : 1;
				distances[i, j] = Math.Min(
					Math.Min(distances[i - 1, j] + 1, distances[i, j - 1] + 1),
					distances[i - 1, j - 1] + cost
				);
			}
		return distances[lngA, lngB];
	}
}