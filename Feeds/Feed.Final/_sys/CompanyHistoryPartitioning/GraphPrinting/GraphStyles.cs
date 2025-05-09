namespace Feed.Final._sys.CompanyHistoryPartitioning.GraphPrinting;

static class GraphStyles
{
	public const string MermaidCss =
		"""
		.timeline-root {
			position: relative;
			background-color: #242536;
		}
		
		.timeline-cell {
			position: absolute;
			background-color: #cfcee1;
			color: #333;
		}
		""";

	public const string MermaidInit =
		"""
		mermaid.initialize({
			startOnLoad: false,
			securityLevel: 'loose',
			theme: 'base',
			themeVariables: {
				primaryColor: '#2c3766',
				fontSize: '12px',
				lineColor: '#cb61bf',
				primaryTextColor: '#fff',
			},
		});
		""";

	public const string MermaidStyles =
		"""
		%% linkStyle default stroke:#000,stroke-width:16px,color:red;
		""";
}