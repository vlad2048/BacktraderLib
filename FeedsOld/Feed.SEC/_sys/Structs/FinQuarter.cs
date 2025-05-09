/*
namespace Feed.SEC._sys.Structs;

public sealed record FinQuarter(int YearEndMonth, int Year, int Quarter) : IComparable<FinQuarter>
{
	public override string ToString() => $"Q{Quarter + 1}'" + $"{Year}"[^2..] + $" (m={YearEndMonth})";

	public static bool DoesDateEndMatch(int yearEndMonth, DateOnly dateEnd)
	{
		if (dateEnd.Day != DateTime.DaysInMonth(dateEnd.Year, dateEnd.Month)) throw new ArgumentException($"Date does not correspond to the end of the month: {dateEnd}");
		var delta = dateEnd.Month - yearEndMonth;
		return delta % 3 == 0;
	}

	public static FinQuarter FromDateEnd(int yearEndMonth, DateOnly dateEnd)
	{
		if (dateEnd.Day != DateTime.DaysInMonth(dateEnd.Year, dateEnd.Month)) throw new ArgumentException($"Date does not correspond to the end of the month: {dateEnd}");
		var delta = dateEnd.Month - yearEndMonth;
		if (delta % 3 != 0) throw new ArgumentException($"Date does not match a quarter (dateEnd:{dateEnd}  yearEndMonth:{yearEndMonth})");
		delta /= 3;
		return delta switch
		{
			<= 0 => new FinQuarter(
				yearEndMonth,
				dateEnd.Year,
				3 + delta
			),
			> 0 => new FinQuarter(
				yearEndMonth,
				dateEnd.Year + 1,
				delta - 1
			),
		};
	}

	// FYE: 0930
	// FY : 2024
	// FP : Q1 | Q2 | Q3 | FY
	public static bool CanParse(string fye, string fy, string fp) =>
		fp is "Q1" or "Q2" or "Q3" or "FY" &&
		fye.Length == 4 &&
		int.TryParse(fye[..2], out _);

	public static FinQuarter Parse(string fye, int fy, string fp)
	{
		var (year, quarter) = fp switch
		{
			"Q1" => (fy, 0),
			"Q2" => (fy, 1),
			"Q3" => (fy, 2),
			"FY" => (fy + 1, 3),
			_ => throw new ArgumentException(),
		};
		return new FinQuarter(
			int.Parse(fye[..2]),
			year,
			quarter
		);
	}

	public DateOnly DateStart => FinQuarterUtils.GetFirstDayAfterMonth(Year - 1, YearEndMonth).AddMonths(3 * Quarter);
	public DateOnly DateEnd => DateStart.AddMonths(3).AddDays(-1);


	public static FinQuarter operator +(FinQuarter q, int v) => v switch
	{
		< 0 => AddNeg(q, -v),
		> 0 => AddPos(q, v),
		_ => q,
	};
	public static FinQuarter operator -(FinQuarter q, int v) => v switch
	{
		< 0 => AddPos(q, -v),
		> 0 => AddNeg(q, v),
		_ => q,
	};

	public static FinQuarter operator ++(FinQuarter q) => q + 1;
	public static FinQuarter operator --(FinQuarter q) => q - 1;

	static FinQuarter AddPos(FinQuarter q, int v)
	{
		if (v < 0) throw new ArgumentException();
		var (year, quart) = (q.Year, q.Quarter);
		for (var i = 0; i < v; i++)
		{
			quart++;
			if (quart > 3)
			{
				quart = 0;
				year++;
			}
		}
		return new FinQuarter(q.YearEndMonth, year, quart);
	}
	static FinQuarter AddNeg(FinQuarter q, int v)
	{
		if (v < 0) throw new ArgumentException();
		var (year, quart) = (q.Year, q.Quarter);
		for (var i = 0; i < v; i++)
		{
			quart--;
			if (quart < 0)
			{
				quart = 3;
				year--;
			}
		}
		return new FinQuarter(q.YearEndMonth, year, quart);
	}



	public int CompareTo(FinQuarter? other)
	{
		if (ReferenceEquals(this, other)) return 0;
		if (ReferenceEquals(null, other)) return 1;
		var yearComparison = Year.CompareTo(other.Year);
		if (yearComparison != 0) return yearComparison;
		var quarterComparison = Quarter.CompareTo(other.Quarter);
		if (quarterComparison != 0) return quarterComparison;
		return YearEndMonth.CompareTo(other.YearEndMonth);
	}
	public static bool operator <(FinQuarter? left, FinQuarter? right) => Comparer<FinQuarter>.Default.Compare(left, right) < 0;
	public static bool operator >(FinQuarter? left, FinQuarter? right) => Comparer<FinQuarter>.Default.Compare(left, right) > 0;
	public static bool operator <=(FinQuarter? left, FinQuarter? right) => Comparer<FinQuarter>.Default.Compare(left, right) <= 0;
	public static bool operator >=(FinQuarter? left, FinQuarter? right) => Comparer<FinQuarter>.Default.Compare(left, right) >= 0;
}





file static class FinQuarterUtils
{
	public static DateOnly GetFirstDayAfterMonth(int year, int month) => GetLastDayInMonth(year, month).AddDays(1);
	public static DateOnly GetLastDayInMonth(int year, int month) => new(year, month, DateTime.DaysInMonth(year, month));
}
*/