using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace BaseUtils;

public static partial class Cmd
{
	public static void EnsureRunning(string exe, string curDir, string[] arguments, string? processName = null) => RunBackground(exe, curDir, arguments, processName);


	sealed class RunningProc(Process proc)
	{
		public Process Proc { get; } = proc;
	}

	static RunningProc RunBackground(string exe, string curDir, string[] arguments, string? processName) => Find(exe, curDir, arguments, processName) ?? Run(exe, curDir, arguments);

	static RunningProc Run(string exe, string curDir, string[] arguments)
	{
		var proc = new Process
		{
			StartInfo = new ProcessStartInfo(exe, arguments)
			{
				WorkingDirectory = curDir,
				CreateNoWindow = true,
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
			}
		};

		var runningProc = CreateRunningProc(proc, exe, curDir, arguments);

		proc.Start();

		var actCmdLine = proc.GetCmdLine();
		var actCurDir = proc.GetCurDir();
		var expCmdLine = $"\"{exe}\" {string.Join(" ", arguments)}";
		var expCurDir = curDir.NormalizeCurDir();

		if (actCmdLine != expCmdLine)
			throw new ArgumentException($"Cmd.EnsureRunning mismatched CmdLine. Act:'{actCmdLine}  Exp:{expCmdLine}'");
		if (actCurDir == null || !actCurDir.StartsWith(expCurDir))
			throw new ArgumentException($"Cmd.EnsureRunning mismatched CurDir. Act:'{actCurDir}  Exp:{expCurDir}'");

		return runningProc;
	}




	static RunningProc? Find(string exe, string curDir, string[] arguments, string? processName)
	{
		var expCurDir = curDir.NormalizeCurDir();
		var expCmdLine = $"\"{exe}\" {string.Join(" ", arguments)}";
		var procs = Process.GetProcessesByName(processName ?? exe)
			.Where(e =>
			{
				var actCmdLine = e.GetCmdLine();
				var actCurDir = e.GetCurDir();
				return actCmdLine == expCmdLine && actCurDir != null && actCurDir.StartsWith(expCurDir);
			})
			.ToArray();

		if (procs.Length == 0)
			return null;

		var proc = procs[0];


		return CreateRunningProc(proc, exe, curDir, arguments);
	}




	static readonly HashSet<string> connectedExes = new();


	static RunningProc CreateRunningProc(Process proc, string exe, string curDir, string[] arguments)
	{
		if (connectedExes.Add(exe))
		{
			var (sbOut, sbErr) = (new StringBuilder(), new StringBuilder());
			proc.EnableRaisingEvents = true;
			proc.OutputDataReceived += (_, args) => sbOut.Append(args.Data);
			proc.ErrorDataReceived += (_, args) => sbErr.Append(args.Data);
			proc.Exited += (_, _) =>
			{
				throw new ArgumentException(
					$"""
					 Process finished unexpectedly
					     Exe   : {exe}
					     CurDir: {curDir}
					     Args  : {string.Join(" ", arguments)}

					 StdOut
					 ------
					 {sbOut}

					 StdErr
					 ------
					 {sbErr}
					 """);
			};
		}
		return new RunningProc(proc);
	}




	static string NormalizeCurDir(this string s) => s.EndsWith('\\') switch
	{
		true => s,
		false => $"{s}\\",
	};
}



file static class ProcUtils
{
	public static string? GetCmdLine(this Process proc)
	{
		var res = ProcessCommandLine.Retrieve(proc, out var cmdLine);
		return res switch
		{
			0 => cmdLine,
			_ => null,
		};
	}

	public static string? GetCurDir(this Process proc) => ProcessUtilities.GetCurrentDirectory(proc);






	enum PROCESSINFOCLASS
	{
		ProcessBasicInformation = 0,
		ProcessWow64Information = 26,
	}
	[Flags]
	enum PEB_OFFSET
	{
		CurrentDirectory,
		//DllPath,
		//ImagePathName,
		CommandLine,
		//WindowTitle,
		//DesktopInfo,
		//ShellInfo,
		//RuntimeData,
		//TypeMask = 0xffff,
		//Wow64 = 0x10000,
	};

	static class Is64BitChecker
	{
		[DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool IsWow64Process(
			[In] IntPtr hProcess,
			[Out] out bool wow64Process
		);

		public static bool GetProcessIsWow64(IntPtr hProcess) =>
			((Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 1) || Environment.OSVersion.Version.Major >= 6) switch
			{
				true => IsWow64Process(hProcess, out var retVal) && retVal,
				false => false,
			};

		public static bool InternalCheckIsWow64()
		{
			if ((Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 1) || Environment.OSVersion.Version.Major >= 6)
			{
				using var p = Process.GetCurrentProcess();
				return IsWow64Process(p.Handle, out var retVal) && retVal;
			}
			else
			{
				return false;
			}
		}
	}

	// All offset values below have been tested on Windows 7 & 8 only
	// but you can use WinDbg "dt ntdll!_PEB" command and search for ProcessParameters offset to find the truth, depending on the OS version
	static class ProcessUtilities
	{
		public static string? GetCurrentDirectory(Process process)
		{
			if (process == null)
				throw new ArgumentNullException(nameof(process));
			return GetCurrentDirectory(process.Id);
		}




		static readonly bool Is64BitProcess = IntPtr.Size > 4;
		static readonly bool Is64BitOperatingSystem = Is64BitProcess || Is64BitChecker.InternalCheckIsWow64();

		static string? GetCurrentDirectory(int processId)
		{
			return GetProcessParametersString(processId, PEB_OFFSET.CurrentDirectory);
		}

		/*static string? GetCommandLine(int processId)
		{
		    return GetProcessParametersString(processId, (PEB_OFFSET)(Is64BitOperatingSystem ? 0x70 : 0x40));
		}*/


		static string? GetProcessParametersString(int processId, PEB_OFFSET Offset)
		{
			var handle = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, false, processId);
			if (handle == IntPtr.Zero)
				throw new Win32Exception(Marshal.GetLastWin32Error());

			var IsWow64Process = Is64BitChecker.InternalCheckIsWow64();
			var IsTargetWow64Process = Is64BitChecker.GetProcessIsWow64(handle);
			var IsTarget64BitProcess = Is64BitOperatingSystem && !IsTargetWow64Process;

			long offset;
			long processParametersOffset = IsTarget64BitProcess ? 0x20 : 0x10;
			switch (Offset)
			{
				case PEB_OFFSET.CurrentDirectory:
					offset = IsTarget64BitProcess ? 0x38 : 0x24;
					break;
				case PEB_OFFSET.CommandLine:
				default:
					return null;
			}

			try
			{
				long pebAddress;
				if (IsTargetWow64Process) // OS : 64Bit, Cur : 32 or 64, Tar: 32bit
				{
					var peb32 = new IntPtr();

					var hr = NtQueryInformationProcess(handle, (int)PROCESSINFOCLASS.ProcessWow64Information, ref peb32, IntPtr.Size, IntPtr.Zero);
					if (hr != 0) throw new Win32Exception(hr);
					pebAddress = peb32.ToInt64();

					var pp = new IntPtr();
					if (!ReadProcessMemory(handle, new IntPtr(pebAddress + processParametersOffset), ref pp, new IntPtr(Marshal.SizeOf(pp)), IntPtr.Zero))
						throw new Win32Exception(Marshal.GetLastWin32Error());

					var us = new UNICODE_STRING_32();
					if (!ReadProcessMemory(handle, new IntPtr(pp.ToInt64() + offset), ref us, new IntPtr(Marshal.SizeOf(us)), IntPtr.Zero))
						throw new Win32Exception(Marshal.GetLastWin32Error());

					if (us.Buffer == 0 || us.Length == 0)
						return null;

					var s = new string('\0', us.Length / 2);
					if (!ReadProcessMemory(handle, new IntPtr(us.Buffer), s, new IntPtr(us.Length), IntPtr.Zero))
						throw new Win32Exception(Marshal.GetLastWin32Error());

					return s;
				}
				else if (IsWow64Process)//Os : 64Bit, Cur 32, Tar 64
				{
					var pbi = new PROCESS_BASIC_INFORMATION_WOW64();
					var hr = NtWow64QueryInformationProcess64(handle, (int)PROCESSINFOCLASS.ProcessBasicInformation, ref pbi, Marshal.SizeOf(pbi), IntPtr.Zero);
					if (hr != 0) throw new Win32Exception(hr);
					pebAddress = pbi.PebBaseAddress;

					long pp = 0;
					hr = NtWow64ReadVirtualMemory64(handle, pebAddress + processParametersOffset, ref pp, Marshal.SizeOf(pp), IntPtr.Zero);
					if (hr != 0)
						throw new Win32Exception(hr);

					var us = new UNICODE_STRING_WOW64();
					hr = NtWow64ReadVirtualMemory64(handle, pp + offset, ref us, Marshal.SizeOf(us), IntPtr.Zero);
					if (hr != 0)
						throw new Win32Exception(hr);

					if (us.Buffer == 0 || us.Length == 0)
						return null;

					var s = new string('\0', us.Length / 2);
					hr = NtWow64ReadVirtualMemory64(handle, us.Buffer, s, us.Length, IntPtr.Zero);
					if (hr != 0)
						throw new Win32Exception(hr);

					return s;
				}
				else// Os,Cur,Tar : 64 or 32
				{
					var pbi = new PROCESS_BASIC_INFORMATION();
					var hr = NtQueryInformationProcess(handle, (int)PROCESSINFOCLASS.ProcessBasicInformation, ref pbi, Marshal.SizeOf(pbi), IntPtr.Zero);
					if (hr != 0) throw new Win32Exception(hr);
					pebAddress = pbi.PebBaseAddress.ToInt64();

					var pp = new IntPtr();
					if (!ReadProcessMemory(handle, new IntPtr(pebAddress + processParametersOffset), ref pp, new IntPtr(Marshal.SizeOf(pp)), IntPtr.Zero))
						throw new Win32Exception(Marshal.GetLastWin32Error());

					var us = new UNICODE_STRING();
					if (!ReadProcessMemory(handle, new IntPtr(pp + offset), ref us, new IntPtr(Marshal.SizeOf(us)), IntPtr.Zero))
						throw new Win32Exception(Marshal.GetLastWin32Error());

					if (us.Buffer == IntPtr.Zero || us.Length == 0)
						return null;

					var s = new string('\0', us.Length / 2);
					if (!ReadProcessMemory(handle, us.Buffer, s, new IntPtr(us.Length), IntPtr.Zero))
						throw new Win32Exception(Marshal.GetLastWin32Error());

					return s;
				}
			}
			finally
			{
				CloseHandle(handle);
			}
		}

		const int PROCESS_QUERY_INFORMATION = 0x400;
		const int PROCESS_VM_READ = 0x10;

		[StructLayout(LayoutKind.Sequential)]
		struct PROCESS_BASIC_INFORMATION
		{
			public IntPtr Reserved1;
			public IntPtr PebBaseAddress;
			public IntPtr Reserved2_0;
			public IntPtr Reserved2_1;
			public IntPtr UniqueProcessId;
			public IntPtr Reserved3;
		}

		[StructLayout(LayoutKind.Sequential)]
		struct UNICODE_STRING
		{
			public short Length;
			public short MaximumLength;
			public IntPtr Buffer;
		}

		// for 32-bit process in a 64-bit OS only
		[StructLayout(LayoutKind.Sequential)]
		struct PROCESS_BASIC_INFORMATION_WOW64
		{
			public long Reserved1;
			public long PebBaseAddress;
			public long Reserved2_0;
			public long Reserved2_1;
			public long UniqueProcessId;
			public long Reserved3;
		}

		// for 32-bit process
		[StructLayout(LayoutKind.Sequential)]
		struct UNICODE_STRING_WOW64
		{
			public short Length;
			public short MaximumLength;
			public long Buffer;
		}

		[StructLayout(LayoutKind.Sequential)]
		struct UNICODE_STRING_32
		{
			public short Length;
			public short MaximumLength;
			public int Buffer;
		}

		[DllImport("ntdll.dll")] static extern int NtQueryInformationProcess(IntPtr ProcessHandle, int ProcessInformationClass, ref PROCESS_BASIC_INFORMATION ProcessInformation, int ProcessInformationLength, IntPtr ReturnLength);

		//ProcessWow64Information, // q: ULONG_PTR
		[DllImport("ntdll.dll")] static extern int NtQueryInformationProcess(IntPtr ProcessHandle, int ProcessInformationClass, ref IntPtr ProcessInformation, int ProcessInformationLength, IntPtr ReturnLength);

		[DllImport("kernel32.dll", SetLastError = true)] static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, ref IntPtr lpBuffer, IntPtr dwSize, IntPtr lpNumberOfBytesRead);

		[DllImport("kernel32.dll", SetLastError = true)] static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, ref UNICODE_STRING lpBuffer, IntPtr dwSize, IntPtr lpNumberOfBytesRead);

		[DllImport("kernel32.dll", SetLastError = true)] static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, ref UNICODE_STRING_32 lpBuffer, IntPtr dwSize, IntPtr lpNumberOfBytesRead);

		//[DllImport("kernel32.dll", SetLastError = true)]
		//private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, ref UNICODE_STRING_WOW64 lpBuffer, IntPtr dwSize, IntPtr lpNumberOfBytesRead);

		[DllImport("kernel32.dll", SetLastError = true)] static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [MarshalAs(UnmanagedType.LPWStr)] string lpBuffer, IntPtr dwSize, IntPtr lpNumberOfBytesRead);

		[DllImport("kernel32.dll", SetLastError = true)] static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

		[DllImport("kernel32.dll")] static extern bool CloseHandle(IntPtr hObject);

		// for 32-bit process in a 64-bit OS only
		[DllImport("ntdll.dll")] static extern int NtWow64QueryInformationProcess64(IntPtr ProcessHandle, int ProcessInformationClass, ref PROCESS_BASIC_INFORMATION_WOW64 ProcessInformation, int ProcessInformationLength, IntPtr ReturnLength);

		[DllImport("ntdll.dll")] static extern int NtWow64ReadVirtualMemory64(IntPtr hProcess, long lpBaseAddress, ref long lpBuffer, long dwSize, IntPtr lpNumberOfBytesRead);

		[DllImport("ntdll.dll")] static extern int NtWow64ReadVirtualMemory64(IntPtr hProcess, long lpBaseAddress, ref UNICODE_STRING_WOW64 lpBuffer, long dwSize, IntPtr lpNumberOfBytesRead);

		[DllImport("ntdll.dll")] static extern int NtWow64ReadVirtualMemory64(IntPtr hProcess, long lpBaseAddress, [MarshalAs(UnmanagedType.LPWStr)] string lpBuffer, long dwSize, IntPtr lpNumberOfBytesRead);
	}
}







file static class ProcessCommandLine
{
	static class Win32Native
	{
		public const uint PROCESS_BASIC_INFORMATION = 0;

		[Flags]
		public enum OpenProcessDesiredAccessFlags : uint
		{
			PROCESS_VM_READ = 0x0010,
			PROCESS_QUERY_INFORMATION = 0x0400,
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct ProcessBasicInformation
		{
			public IntPtr Reserved1;
			public IntPtr PebBaseAddress;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
			public IntPtr[] Reserved2;
			public IntPtr UniqueProcessId;
			public IntPtr Reserved3;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct UnicodeString
		{
			public ushort Length;
			public ushort MaximumLength;
			public IntPtr Buffer;
		}

		// This is not the real struct!
		// I faked it to get ProcessParameters address.
		// Actual struct definition:
		// https://learn.microsoft.com/en-us/windows/win32/api/winternl/ns-winternl-peb
		[StructLayout(LayoutKind.Sequential)]
		public struct PEB
		{
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
			public IntPtr[] Reserved;
			public IntPtr ProcessParameters;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct RtlUserProcessParameters
		{
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
			public byte[] Reserved1;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
			public IntPtr[] Reserved2;
			public UnicodeString ImagePathName;
			public UnicodeString CommandLine;
		}

		[DllImport("ntdll.dll")]
		public static extern uint NtQueryInformationProcess(
			IntPtr ProcessHandle,
			uint ProcessInformationClass,
			IntPtr ProcessInformation,
			uint ProcessInformationLength,
			out uint ReturnLength);

		[DllImport("kernel32.dll")]
		public static extern IntPtr OpenProcess(
			OpenProcessDesiredAccessFlags dwDesiredAccess,
			[MarshalAs(UnmanagedType.Bool)] bool bInheritHandle,
			uint dwProcessId);

		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool ReadProcessMemory(
			IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer,
			uint nSize, out uint lpNumberOfBytesRead);

		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CloseHandle(IntPtr hObject);
	}

	static bool ReadStructFromProcessMemory<TStruct>(IntPtr hProcess, IntPtr lpBaseAddress, out TStruct val) where TStruct: struct
	{
		val = default;
		var structSize = Marshal.SizeOf<TStruct>();
		var mem = Marshal.AllocHGlobal(structSize);
		try
		{
			if (Win32Native.ReadProcessMemory(
				hProcess, lpBaseAddress, mem, (uint)structSize, out var len) &&
				len == structSize)
			{
				val = Marshal.PtrToStructure<TStruct>(mem);
				return true;
			}
		}
		finally
		{
			Marshal.FreeHGlobal(mem);
		}
		return false;
	}

	public static int Retrieve(Process process, out string? commandLine)
	{
		int rc;
		commandLine = null;
		var hProcess = Win32Native.OpenProcess(
			Win32Native.OpenProcessDesiredAccessFlags.PROCESS_QUERY_INFORMATION |
			Win32Native.OpenProcessDesiredAccessFlags.PROCESS_VM_READ, false, (uint)process.Id);
		if (hProcess != IntPtr.Zero)
		{
			try
			{
				var sizePBI = Marshal.SizeOf<Win32Native.ProcessBasicInformation>();
				var memPBI = Marshal.AllocHGlobal(sizePBI);
				try
				{
					var ret = Win32Native.NtQueryInformationProcess(
						hProcess, Win32Native.PROCESS_BASIC_INFORMATION, memPBI,
						(uint)sizePBI, out _);
					if (0 == ret)
					{
						var pbiInfo = Marshal.PtrToStructure<Win32Native.ProcessBasicInformation>(memPBI);
						if (pbiInfo.PebBaseAddress != IntPtr.Zero)
						{
							if (ReadStructFromProcessMemory<Win32Native.PEB>(hProcess,
								pbiInfo.PebBaseAddress, out var pebInfo))
							{
								if (ReadStructFromProcessMemory<Win32Native.RtlUserProcessParameters>(
									hProcess, pebInfo.ProcessParameters, out var ruppInfo))
								{
									var clLen = ruppInfo.CommandLine.MaximumLength;
									var memCL = Marshal.AllocHGlobal(clLen);
									try
									{
										if (Win32Native.ReadProcessMemory(hProcess,
											ruppInfo.CommandLine.Buffer, memCL, clLen, out _))
										{
											commandLine = Marshal.PtrToStringUni(memCL) ?? throw new ArgumentException("Shouldn't be null");
											rc = 0;
										}
										else
										{
											// couldn't read command line buffer
											rc = -6;
										}
									}
									finally
									{
										Marshal.FreeHGlobal(memCL);
									}
								}
								else
								{
									// couldn't read ProcessParameters
									rc = -5;
								}
							}
							else
							{
								// couldn't read PEB information
								rc = -4;
							}
						}
						else
						{
							// PebBaseAddress is null
							rc = -3;
						}
					}
					else
					{
						// NtQueryInformationProcess failed
						rc = -2;
					}
				}
				finally
				{
					Marshal.FreeHGlobal(memPBI);
				}
			}
			finally
			{
				Win32Native.CloseHandle(hProcess);
			}
		}
		else
		{
			// couldn't open process for VM read
			rc = -1;
		}
		return rc;
	}
}