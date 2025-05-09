namespace BacktraderLib;

public sealed class JSRunException(JSErrorType type, string message, Exception? innerException)
	: Exception(message, innerException)
{
	public JSErrorType Type { get; } = type;
}
