namespace Inventory.Extensions;

using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

public static class ResultExtensions 
{
    public static IResult ToProblematic(this Result result)
    {
        if (result.IsSuccess)
            throw new InvalidOperationException("Result was successful.");

        var errorMessages = result.Errors.Select(e => e.Message).ToArray();

        // Fetch the first error
        var firstError = result.Errors.OfType<Error>().FirstOrDefault();

        if (firstError is null)
            return Results.Problem(
                    statusCode: 500,
                    title: "Unknown error occurred!"
                );

        var statusCode = GetStatusCode(firstError.errorType);

        return Results.Problem(
                    statusCode: statusCode,
                    title: GetTitle(firstError.errorType),
                    detail: firstError.Description ?? "An error occurred during the operation.",
                    extensions: new Dictionary<string, object?>
                    {
                        // Provide a default value if {errorCode} is null
                        { "errorCode", firstError.Code ?? "UNKNOWN"},
                        { "errorType", firstError.errorType },
                        { "description", firstError.Description },
                        { "errors", errorMessages }
                    }
                );
    }

    public static ProblemDetails ToProblematicAction(this Result result)
    {
        if (result.IsSuccess)
            throw new InvalidOperationException("Result was successful.");

        var errorMessages = result.Errors.Select(e => e.Message).ToArray();

        // Fetch the first error
        var firstError = result.Errors.OfType<Error>().FirstOrDefault();

        if (firstError is null)
        { 
             return new ProblemDetails
            {
                Title = "Unknown error occurred!",
                Status = 500
            };
        }
            

        var statusCode = GetStatusCode(firstError.errorType);

        return new ProblemDetails
        {
            Status = statusCode,
            Title = GetTitle(firstError.errorType),
            Detail = firstError.Description ?? "An error occurred during the operation.",
            Extensions = new Dictionary<string, object?>
                    {
                        // Provide a default value if {errorCode} is null
                        { "errorCode", firstError.Code ?? "UNKNOWN"},
                        { "errorType", firstError.errorType },
                        { "description", firstError.Description },
                        { "errors", errorMessages }
                    }
        };
    }

    private static int GetStatusCode(ErrorType errorType) =>
        errorType switch
        {
            ErrorType.Failure => StatusCodes.Status500InternalServerError,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError      
        };    


    private static string GetTitle(ErrorType errorType) =>
        errorType switch
        {
            ErrorType.Failure => "Failure",
            ErrorType.Validation => "Validation",
            ErrorType.NotFound => "Not Found",
            ErrorType.Conflict => "Conflict",
            _ => "Server Failure",   
        };    

}