using BacktraderLib._sys;

namespace BacktraderLib;

public static partial class JS
{
	internal static bool ErrorDumpingDisabled { get; set; }


	internal static void Init() => JSInit.Init();


	public static void DisableErrorDumping() => ErrorDumpingDisabled = true;
}