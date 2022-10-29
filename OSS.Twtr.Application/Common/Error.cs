namespace OSS.Twtr.Core;

public readonly record struct Error(string Message, string ErrorCode = "GeneralError", int Severity = 0);