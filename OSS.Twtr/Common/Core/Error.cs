namespace OSS.Twtr.Common.Core;

public readonly record struct Error(string Message, ErrorCode ErrorCode = ErrorCode.Unexpected, ErrorSeverity ErrorSeverity = ErrorSeverity.Error);

public enum ErrorCode
{
    BadRequest = 400,
    Unauthorized = 401,
    Forbidden = 403,
    NotFound = 404,
    Conflict = 409,
    Validation = 428,
    Unexpected = 500,
}

public enum ErrorSeverity
{
    Error,
    Warning,
    Info,
}