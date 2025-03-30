using NUnit.Framework.Legacy;
using System.Text;
using BaseUtils;
using Feed.SEC._sys.Logic;

namespace Feed.SEC.Tests.Utils;

class LineParserTests
{
	[Test]
	public void FromDatasets_0()
	{
		"'The value. ''Part noncash'' refers to.'	Debt Conversion".Parse([
			"The value. 'Part noncash' refers to.",
			"Debt Conversion",
		]);
	}

	[Test]
	public void FromDatasets_1()
	{
		"'AB''CD''	DEF'	vlad".Parse([
			"AB'CD'	DEF",
			"vlad",
		]);
	}

	[Test]
	public void FromDatasets_2()
	{
		"0001193125-09-102866	IncomeTaxExpenseBenefit	us-gaap/2008	20090331	1	USD			25000000.0000	'After giving retroactive effect to the adoption of Staff Position No. APB 14-1, ''Accounting for Convertible Debt Instruments That May Be Settled in Cash upon Conversion (Including Partial Cash Settlement)'', as further described in note 3.'".Parse([
			"0001193125-09-102866",
			"IncomeTaxExpenseBenefit",
			"us-gaap/2008",
			"20090331",
			"1",
			"USD",
			"",
			"",
			"25000000.0000",
			"After giving retroactive effect to the adoption of Staff Position No. APB 14-1, 'Accounting for Convertible Debt Instruments That May Be Settled in Cash upon Conversion (Including Partial Cash Settlement)', as further described in note 3.",
		]);
	}

	[Test]
	public void FromDatasets_3()
	{
		"vlad	'inside ''inner'''".Parse([
			"vlad",
			"inside 'inner'",
		]);
	}

	[Test]
	public void FromDatasets_4()
	{
		"MinorityInterestIncreaseFromCapitalContributionsToNoncontrollingInterestHolders	0000950103-09-002146	1	0	monetary	D	C	'Capital contribution attributable to noncontrolling interest in Jerini AG (''Jerini'')'	Increase in noncontrolling interest balance from capital contributions attributable to noncontrolling interest holders.".Parse([
			"MinorityInterestIncreaseFromCapitalContributionsToNoncontrollingInterestHolders",
			"0000950103-09-002146",
			"1",
			"0",
			"monetary",
			"D",
			"C",
			"Capital contribution attributable to noncontrolling interest in Jerini AG ('Jerini')",
			"Increase in noncontrolling interest balance from capital contributions attributable to noncontrolling interest holders.",
		]);
	}

	
	[Test]
	public void ContainTabs()
	{
		"vlad	other	'ola	tabby	tab'".Parse([
			"vlad",
			"other",
			"ola	tabby	tab",
		]);
	}

	[Test]
	public void DoubleQuotes()
	{
		"vlad	other	'long ''one'', end'".Parse([
			"vlad",
			"other",
			"long 'one', end",
		]);
	}
}


file static class CheckUtils
{
	public static void Parse(this string str, string[] exp)
	{
		str = str.ReplaceQuotes();
		exp = exp.SelectA(ReplaceQuotes);

		var act = LineIO.Line2Fields(str, null, LineMethod.OriginalEscaping);
		CollectionAssert.AreEqual(exp, act);
	}

	static string ReplaceQuotes(this string e)
	{
		if (e.Contains("\"")) throw new ArgumentException("Use single quotes to make the tests readable");
		return e.Replace("'", "\"");
	}
}