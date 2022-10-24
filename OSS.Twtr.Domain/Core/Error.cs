namespace OSS.Twtr.Domain;

public record struct Error(string Message, string ErrorCode = "GeneralError", int Severity = 0);