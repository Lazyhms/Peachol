using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Mvc.Filters;

public sealed class GlobalExceptionFilterAttribute(
    IWebHostEnvironment webHostEnvironment,
    IOptionsSnapshot<GlobalResult> options,
    ILogger<GlobalExceptionFilterAttribute> logger) : ExceptionFilterAttribute
{
    private readonly ILogger<GlobalExceptionFilterAttribute> _logger = logger;

    private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;

    private readonly GlobalResult _globalExceptionResult = options.Get("Global_Exception");

    private readonly GlobalResult _businessExceptionResult = options.Get("Biz_Exception");

    public override Task OnExceptionAsync(ExceptionContext context)
    {
        switch (context.Exception)
        {
            case BizException businessException:
                _logger.LogError(businessException, "Title:BizException HResult:{HResult}", businessException.HResult);

                context.ExceptionHandled = true;
                _businessExceptionResult.Message = businessException.Message;
                context.Result = new ObjectResult(_businessExceptionResult);
                break;
            case Exception handledException:
                _logger.LogError(handledException, "Title:Exception HResult:{HResult}", handledException.HResult);

                context.ExceptionHandled = true;
                _globalExceptionResult.Message = _webHostEnvironment.IsDevelopment()
                    ? handledException.ToString() : "Internal server error, Please contact the administrator";
                context.Result = new ObjectResult(_globalExceptionResult);
                break;
        }

        return base.OnExceptionAsync(context);
    }
}