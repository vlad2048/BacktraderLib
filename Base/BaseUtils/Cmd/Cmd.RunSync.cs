using System.Diagnostics;

namespace BaseUtils;


public sealed record ProcRes(string Exe, string CurDir, string[] arguments, string StdOut, string StdErr, int ExitCode)
{
	public bool IsSuccess => ExitCode == 0;
}

public static class ProcResExt
{
	public static ProcRes EnsureSuccess(this ProcRes res)
	{
		// @formatter:off
		if (!res.IsSuccess)
			throw new Exception(
			$"""
			Process failed
			==============
			Exe: {res.Exe}
			CurDir: {res.CurDir}
			Arguments: {string.Join(" ", res.arguments)}
			
			ExitCode: {res.ExitCode}
			
			StdOut:
			-------
			{res.StdOut}
			
			StdErr:
			-------
			{res.StdErr}
			""");
		// @formatter:on
		return res;
	}

	// @formatter:off
	public static void LogToConsole(this ProcRes res) => Console.WriteLine(
	$"""
	Process result
	==============
	Exe: {res.Exe}
	CurDir: {res.CurDir}
	Arguments: {string.Join(" ", res.arguments)}
	
	ExitCode: {res.ExitCode}
	
	StdOut:
	-------
	{res.StdOut}
	
	StdErr:
	-------
	{res.StdErr}
	""");
	// @formatter:on
}



public static partial class Cmd
{
	public static ProcRes RunSync(string exe, string curDir, string[] arguments, bool showInConsole = false)
	{
		var proc = new Process
		{
			StartInfo = new ProcessStartInfo(exe, arguments)
			{
				WorkingDirectory = curDir,
				CreateNoWindow = false,
				UseShellExecute = false,
				RedirectStandardOutput = !showInConsole,
				RedirectStandardError = !showInConsole,
			}
		};
		proc.Start();
		proc.WaitForExit();
		return new ProcRes(
			exe,
			curDir,
			arguments,
			showInConsole ? string.Empty : proc.StandardOutput.ReadToEnd(),
			showInConsole ? string.Empty : proc.StandardError.ReadToEnd(),
			proc.ExitCode
		);
	}
}