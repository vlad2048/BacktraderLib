using System.Diagnostics;

namespace BacktraderReverser.Utils;

static class Python
{
	public static string[] Run(string code)
	{
		if (File.Exists(Consts.PythonFile)) File.Delete(Consts.PythonFile);
		File.WriteAllText(Consts.PythonFile, code);

		var procNfo = new ProcessStartInfo("python", Consts.PythonFile)
		{
			UseShellExecute = false,
			CreateNoWindow = true,
			RedirectStandardOutput = true,
			RedirectStandardError = true,
		};
		var proc = Process.Start(procNfo);
		proc.WaitForExit();
		var (stdErr, stdOut) = (proc.StandardError.ReadToEnd(), proc.StandardOutput.ReadToEnd());
		if (proc.ExitCode != 0)
		{
			throw new ArgumentException(
				$"""
			    Error running Python code
			    =========================
			        File     : {Consts.PythonFile}
			        Exit code: {proc.ExitCode}
			        
			    StandardError
			    -------------
			    {stdErr}
			        
			    StandardOutput
			    --------------
			    {stdOut}
			    """);
		}

		return stdOut.Split(Environment.NewLine);
	}
}