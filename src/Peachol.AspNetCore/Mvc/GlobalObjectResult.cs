namespace Microsoft.AspNetCore.Mvc;

public class GlobalObjectResult : GlobalResult
{
    public object? Value { get; set; }
}
