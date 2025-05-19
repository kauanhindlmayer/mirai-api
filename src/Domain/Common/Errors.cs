using ErrorOr;

namespace Domain.Common;

public static class Errors
{
    public static Error InvalidSort(string? sort) => Error.Validation(
        code: "Sort.Invalid",
        description: $"The sort parameter '{sort}' is not valid. Please use a valid sort field.");
}
