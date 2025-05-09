<Query Kind="Program">
  <Namespace>LINQPadPlus</Namespace>
  <Namespace>LINQPad.Controls</Namespace>
</Query>

#load ".\lib"
void OnStart() => LINQPadPlusSetup.Init();	// Always initialize the library in OnStart()


void Main()
{
	// JS.Run()
	JSRun_Examples.Run();

	// JS.Return()
	JSReturn_Examples.Run();
	
	// Syntax highlighting
	// If you use these functions in VisualStudio with Resharper, you will get JavaScript syntax highlighting
	// However, if you use string interpolation, it will break the highlighting. As a workaround you can do this:
	JS.Run($"console.log('{123}')");										// No highligthing
	JS.Run("console.log('____0____')", e => e.JSRepl_Var(0, "123"));        // Workaround (there are a few functions similar to JSRepl_Var for different formatting options)

	// Run when element appears in the DOM
	var div = new Span("Initial text");
	div.HtmlElement.ID = "myid";
	JS.RunOn("myid", "elt => elt.innerText = 'Modified text'");
	div.Dump();
}


static class JSRun_Examples
{
	public static void Run()
	{
		Valid();
		CompilationError();
		RuntimeError();
	}
	
	
	// Create a button in JavaScript (valid code)
	static void Valid() => JS.Run(
		"""
		const button = document.createElement('button');
		button.textContent = 'Run_JS_Valid';
		document.body.appendChild(button);
		"""
	);

	// Compilation error will trigger an exception with all the details to debug it
	static void CompilationError() => JS.Run(
		"""
		const button = document.createElement('button');
		{
		button.textContent = 'Run_JS_CompilationError';
		document.body.appendChild(button);
		"""
	);

	// Runtime error will trigger an exception with all the details to debug it
	static void RuntimeError() => JS.Run(
		"""
		const button = document.createElement('button');
		throw new Error('Oops');
		button.textContent = 'Run_JS_RuntimeError';
		document.body.appendChild(button);
		"""
	);
}





static class JSReturn_Examples
{
	public static void Run()
	{
		Valid_Expression().Dump();
		Valid_Function().Dump();
		ForgotToInvokeIIF().Dump();
		NoIIF().Dump();
		CannotReturnEmptyObject().Dump();
	}
	
	
	// An expression is valid
	static string Valid_Expression() => JS.Return(
		"""
		123
		"""
	);

	// For more complex code, use an IIF (Immediately Invoked Function)
	static string Valid_Function() => JS.Return(
		"""
		(function() {
			return 456;
		})();
		"""
	);

	// If you forget to invoke the IIF, a relevant exception will be thrown
	static string ForgotToInvokeIIF() => JS.Return(
		"""
		(function() {
			return 456;
		});
		"""
	);

	// An easy mistake to make is to forget to wrap return statements in an IIF, an exception is thrown
	static string NoIIF() => JS.Return(
		"""
		return 789;
		"""
	);

	// Unfortunately, because of the tricks used, you cannot return an empty object (as this is mistaken for forgetting to invoke an IIF), an exception is thrown
	static string CannotReturnEmptyObject() => JS.Return(
		"""
		(function() {
			return {};
		})();
		"""
	);
}








