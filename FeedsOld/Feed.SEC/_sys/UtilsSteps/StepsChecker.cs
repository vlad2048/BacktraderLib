using Feed.SEC._sys.Logic;

namespace Feed.SEC._sys.UtilsSteps;

static class StepsChecker
{
	public static void Check(Step step)
	{
		if (step.HasFlag(Step.Download)) Check_Download();
		if (step.HasFlag(Step.Clean)) Check_Clean();
		if (step.HasFlag(Step.Group)) Check_Group();
	}


	static void Check_Download()
	{
		var archFiles = Consts.Download.GetAllZipFiles();
		foreach (var archFile in archFiles)
		{
			RowArchiveChecker.CheckAsStringyRows(archFile, LineMethod.OriginalEscaping);
		}
		Console.WriteLine("Download -> OK");
	}


	static void Check_Clean()
	{
		var archFiles = Consts.Clean.GetAllZipFiles();
		foreach (var archFile in archFiles)
		{
			//RowArchiveChecker.CheckAsStringyRows(archFile, LineMethod.ReplaceTabChar);
			RowArchiveChecker.CheckAsNormalRows(archFile);
		}
		Console.WriteLine("Clean    -> OK");
	}

	static void Check_Group()
	{
		var archFiles = Consts.Group.GetAllCompanyZipFiles();
		for (var index = 0; index < archFiles.Length; index++)
		{
			var archFile = archFiles[index];
			//RowArchiveChecker.CheckAsStringyRows(archFile, LineMethod.ReplaceTabChar);
			RowArchiveChecker.CheckAsNormalRows(archFile);
			if (index % 100 == 0)
				Console.WriteLine($"    Progress {index + 1}/{archFiles.Length} ");
		}

		Console.WriteLine("Group    -> OK");
	}
}