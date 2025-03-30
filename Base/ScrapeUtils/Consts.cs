namespace ScrapeUtils;

static class Consts
{
	public const string ChromeUserDataFolder = @"C:\Users\admin\AppData\Local\Google\Chrome\User Data";


	public const bool DumpFormattedScriptWhenRunning = false;

	public const string ScriptUsings =
		"""
		using System;
		using System.Collections;
		using System.Collections.Generic;
		using System.IO;
		using System.Linq;
		using System.Linq.Expressions;
		using System.Threading;
		using System.Threading.Tasks;
		using System.Text.RegularExpressions;
		using Microsoft.Playwright;
		
		
		""";
}