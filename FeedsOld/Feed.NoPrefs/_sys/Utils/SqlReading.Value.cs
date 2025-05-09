using MySqlConnector;

namespace Feed.NoPrefs._sys.Utils;

static partial class SqlReading
{
	public static T? ReadValue<T>(this MySqlConnection conn, string sql)
	{
		try
		{
			using var cmd = new MySqlCommand(sql, conn);
			using var reader = cmd.ExecuteReader();
			var dbCol = reader.GetDbSchema().Single();
			if (!reader.HasRows) throw new ArgumentException("No rows returned");
			reader.Read();

			var obj = reader.Map(dbCol, typeof(T));

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
			var res = (T)obj;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
			return res;
		}
		catch (MySqlException ex)
		{
			throw new SqlReadingException(sql, ex);
		}
	}
}