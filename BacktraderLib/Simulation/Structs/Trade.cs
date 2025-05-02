using BacktraderLib._sys;

namespace BacktraderLib;


public sealed record Trade(
	int Id,
	int DadId,
	string Symbol,
	TradeDir Dir,
	double Size,
	bool IsOpen,

	int EntrDate,
	double EntrPrice,
	double EntrComm,

	int ExitDate,
	double ExitPrice,
	double ExitComm
)
{
	public double Pnl => Dir.ToSign() * Size * (ExitPrice - EntrPrice) - (EntrComm + ExitComm);

	public object ToDump() => new
	{
		Id = $"{Id}" + (DadId != Id) switch
		{
			true => $" (dad:{DadId})",
			false => string.Empty,
		},
		Symbol,
		Dir,
		Size = Size.FmtDumpVal(),
		IsOpen,

		EntrDate,
		EntrPrice = EntrPrice.FmtDumpVal(),
		EntrComm = EntrComm.FmtDumpVal(),

		ExitDate,
		ExitPrice = ExitPrice.FmtDumpVal(),
		ExitComm = ExitComm.FmtDumpVal(),

		Pnl = Pnl.FmtDumpVal(),
	};
}


public enum TradeDir
{
	Long = 1,
	Shrt = 2,
}
