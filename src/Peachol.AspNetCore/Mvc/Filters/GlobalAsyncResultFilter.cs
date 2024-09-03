using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Mvc.Filters;

public sealed class GlobalAsyncResultFilter(IOptionsSnapshot<GlobalObjectResult> options) : IAsyncResultFilter
{
    private readonly GlobalObjectResult _objectObjectResult = options.Get("Global_Success");

    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        switch (context.Result)
        {
            case EmptyResult:
                context.Result = new ObjectResult(_objectObjectResult);
                break;
            case ContentResult contentResult:
                _objectObjectResult.Data = contentResult.Content;
                context.Result = new ObjectResult(_objectObjectResult);
                break;
            case JsonResult jsonResult:
                _objectObjectResult.Data = jsonResult.Value;
                context.Result = new JsonResult(_objectObjectResult);
                break;
            case ObjectResult objectResult:
                _objectObjectResult.Data = objectResult.Value;
                context.Result = new ObjectResult(_objectObjectResult);
                break;
        }
        await next();
    }
}