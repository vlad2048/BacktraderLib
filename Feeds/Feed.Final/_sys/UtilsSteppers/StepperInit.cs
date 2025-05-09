using LINQPad;

namespace Feed.Final._sys.UtilsSteppers;

static class StepperInit
{
	public static void Init()
	{
		Util.HtmlHead.AddStyles(
			"""
			.stepper-sidebyside {
				display: flex;
				column-gap: 50px;
			}
			
			
			
			.stepper {
				display: flex;
				flex-direction: column;
				align-items: stretch;
			}
			
			.stepper > h1 {
				font-size: 22px;
				font-weight: bold;
			}
			
			.stepper-table {
				display: inline-grid;
				grid-template-columns: fit-content(600px) 60px min-content;
			}
			
			
			
			.stepper-name {
			}
			
			.stepper-sign {
				justify-self: right;
			}
			
			.stepper-value {
				text-align: right;
				white-space: pre;
			}
			
			.stepper-total {
				border-top: 1px solid #ffffff;
			}
			"""
		);
	}
}