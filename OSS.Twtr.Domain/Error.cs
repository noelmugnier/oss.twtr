namespace OSS.Twtr.Core;

public record struct Error(string Message, string ErrorCode = "GeneralError", int Severity = 0);