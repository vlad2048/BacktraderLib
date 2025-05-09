using BacktraderLib;
using BaseUtils;

namespace Feed.Final._sys.UtilsSteppers;

static class StepperDumper
{
	public static Tag ToTag(this Stepper stepper)
	{
		var titleTag = new Tag("h3", null, stepper.Title);
		
		var lineTags = stepper.Steps.SelectManyA(e => new[]
		{
			e.View switch
			{
				null => new Tag("span", null, e.GetNameStr())
				{
					Class = "stepper-name",
				},
				
				not null => new Tag("a", null, e.GetNameStr())
				{
					Class = "stepper-name",
					OnClick = () =>
					{
						e.View();
					},
				},
			},
			
			new("span", null, e.GetSignStr())
			{
				Class = "stepper-sign",
			},
			new("span", null, e.GetNumStr())
			{
				Class = "stepper-value" + (e.Type is StepType.Total ? " stepper-total" : ""),
			},
		});

		var tableTag = new Tag("div")
		{
			Class = "stepper-table",
			Kids = lineTags,
		};


		return new Tag("div")
		{
			Class = "stepper",
			Kids =
			[
				titleTag,
				tableTag,
			],
		};
	}

	static string GetNameStr(this Step step) =>
		step.Type switch
		{
			StepType.Plus or StepType.Minus => step.Name,
			StepType.Total => "Total",
			_ => throw new ArgumentOutOfRangeException(),
		};
	
	static string GetSignStr(this Step step) =>
		step.Type switch
		{
			StepType.Minus => "-",
			StepType.Total => "=",
			_ => " ",
		};

	static string GetNumStr(this Step step) =>
		$"{step.Count}";
}