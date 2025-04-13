global using System;
global using System.Collections.Generic;
global using System.IO;
global using System.Linq;
global using System.Net.Http;
global using System.Threading;
global using System.Threading.Tasks;
global using static WebScript.Globals;
using Microsoft.Playwright;

namespace WebScript;

public static class Globals
{
	public static IPage Page { get; set; }
	public static ITracing Tracing { get; set; }
	public static Action<object> log { get; set; }

	public static void Log(object obj)
	{
		obj = obj switch
		{
			string str => $"[{DateTime.Now:HH:mm:ss.fff}] {str}",
			_ => obj,
		};
		log(obj);
	}

	public static void LogTitle(string str)
	{
		Log(str);
		Log(new string('=', str.Length));
	}
	public static async Task Pause(double seconds) => await Task.Delay(TimeSpan.FromSeconds(seconds));
}