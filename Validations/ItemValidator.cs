using FluentResults;

public static class ItemValidator
{
    public static Result Validate(Item item)
    {
        var result = new Result();

        if (string.IsNullOrWhiteSpace(item.Name))
            result.WithError("Name is required.");

        if (item.Price < 0)
            result.WithError("Price cannot be negative.");

        if (item.Quantity < 0)
            result.WithError("Quantity cannot be negative.");

        return result;
    }
}