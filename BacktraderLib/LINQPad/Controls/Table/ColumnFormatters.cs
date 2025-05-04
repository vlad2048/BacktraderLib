using JetBrains.Annotations;

namespace BacktraderLib;

public static class ColumnFormatters
{
	[LanguageInjection(InjectedLanguage.JAVASCRIPT)]
	public const string Money =
		"""
		function(cell) {
			let v = cell.getValue();
			let sym = '';
			if (v >= 1_000_000_000) {
			    v /= 1_000_000_000;
			    sym = ' B';
			} else if (v >= 1_000_000) {
			    v /= 1_000_000;
			    sym = ' M';
			} else if (v >= 1_000) {
			    v /= 1_000;
			    sym = ' K';
			}
			const vStr = v.toLocaleString('en-US', {
			    minimumFractionDigits: 2,
			    maximumFractionDigits: 2,
			});
			return `$${vStr}${sym}`;
		}
		""";
}