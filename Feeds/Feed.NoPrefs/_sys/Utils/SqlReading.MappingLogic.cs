using MySqlConnector;

namespace Feed.NoPrefs._sys.Utils;

static partial class SqlReading
{
	static object? Map(
		this MySqlDataReader reader,
		DbCol srcCol,
		Type dstType
	)
	{
		var idx = srcCol.Idx;
		var srcType = srcCol.CsType;
		if (reader.IsDBNull(idx))
		{
			if (srcType == typeof(decimal) && dstType != typeof(decimal?)) throw new ArgumentException("NULL found in decimal column. The property type needs to be decimal? and not decimal.");
			return null;
		}

		string MkMapErr(string supported) =>
		$"""
		src:{srcType.Name} -> dst:{dstType.Name} mapping not supported"
		Only dst supported are: {supported}
		
		srcCol.Name     : {srcCol.Name}
		srcCol.DbType   : {srcCol.DbType}
		srcCol.AllowNull: {srcCol.AllowNull}
		
		dstType         : {dstType.Name}
		""";

		if (srcType == typeof(DateTime))
		{
			if (dstType != typeof(DateTime)) throw new ArgumentException(MkMapErr("DateTime"));
			return reader.GetDateTime(idx);
		}

		if (srcType == typeof(string))
		{
			if (dstType != typeof(string)) throw new ArgumentException(MkMapErr("string"));
			return reader.GetString(idx);
		}

		if (srcType == typeof(decimal))
		{
			if (dstType != typeof(decimal) && dstType != typeof(decimal?)) throw new ArgumentException(MkMapErr("decimal, decimal?"));
			return reader.GetDecimal(idx);
		}

		if (srcType == typeof(long))
		{
			if (dstType != typeof(long)) throw new ArgumentException(MkMapErr("long"));
			return reader.GetInt64(idx);
		}

		if (srcType == typeof(int))
		{
			if (dstType != typeof(int)) throw new ArgumentException(MkMapErr("int"));
			return reader.GetInt32(idx);
		}

		if (srcType == typeof(sbyte))
		{
			if (dstType != typeof(bool) && dstType != typeof(bool?)) throw new ArgumentException(MkMapErr("bool, bool?"));
			var val = reader.GetSByte(idx);
			if (val != 0 && val != 1) throw new ArgumentException($"sbyte value can only be 0 or 1 to map to bool (but it's: '{val}')");
			return val == 1;
		}

		throw new ArgumentException(
		$"""
		Mapping from src:{srcType.Name} not supported
		
		srcCol.Name     : {srcCol.Name}
		srcCol.DbType   : {srcCol.DbType}
		srcCol.AllowNull: {srcCol.AllowNull}
		
		dstType         : {dstType.Name}
		""");
	}
}