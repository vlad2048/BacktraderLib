namespace LINQPadPlus._sys.Structs;


interface IJSError;
sealed record JSRuntimeError(string Id, string Message, string Stack) : IJSError;
sealed record JSCompilationError(string Message, Exception? InnerException) : IJSError;
sealed record JSWrongReturnTypeError(string Message) : IJSError;
sealed record JSUnknownError : IJSError;
