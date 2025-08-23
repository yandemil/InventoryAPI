namespace Inventory.Extensions;

using FluentResults;

public class Error : FluentResults.Error
{
    public string Code { get; set; }
    public string Description { get; set; }
    public ErrorType errorType { get; }

    private Error(string code, string description, ErrorType error)
    {
        Code = code;
        Description = description;
        errorType = error;
        Metadata.Add("Code", code);
        Metadata.Add("ErrorType", error.ToString());
    }

    public static Error Failure(string code, string description) =>
        new Error(code, description, ErrorType.Failure);

    public static Error NotFound(string code, string description) =>
        new Error(code, description, ErrorType.NotFound);

    public static Error Validation(string code, string description) =>
        new Error(code, description, ErrorType.Validation);

    public static Error Conflict(string code, string description) =>
        new Error(code, description, ErrorType.Conflict);
}

public enum ErrorType
{
    Failure = 0,        // Generic error
    Validation = 1,     // Input or business rule violation
    NotFound = 2,       // Missing resource
    Conflict = 3        // Resource state conflict
}