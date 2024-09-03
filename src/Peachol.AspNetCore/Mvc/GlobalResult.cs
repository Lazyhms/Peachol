namespace Microsoft.AspNetCore.Mvc;

public class GlobalResult
{
    public int Code { get; set; }

    public string Message { get; set; } = default!;
}

public class GlobalObjectResult : GlobalResult
{
    public object? Data { get; set; }
}
