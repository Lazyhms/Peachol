using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Mvc.Filters;

public sealed class GlobalAsyncExceptionFilter(
    IWebHostEnvironment webHostEnvironment,
    IOptionsSnapshot<GlobalResult> options,
    ILogger<GlobalAsyncExceptionFilter> logger) : IAsyncExceptionFilter
{
    private readonly ILogger<GlobalAsyncExceptionFilter> _logger = logger;

    private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;

    private readonly GlobalResult _globalExceptionResult = options.Get("Global_Exception");

    private readonly GlobalResult _businessExceptionResult = options.Get("Business_Exception");

    public async Task OnExceptionAsync(ExceptionContext context)
    {
        switch (context.Exception)
        {
            case BusinessException businessException:
                _logger.LogError(businessException, "Title:业务异常 HResult:{HResult}", businessException.HResult);

                context.ExceptionHandled = true;
                _businessExceptionResult.Message = businessException.Message;
                context.Result = new ObjectResult(_businessExceptionResult);
                break;
            case Exception handledException:
                _logger.LogError(handledException, "Title:系统异常 HResult:{HResult}", handledException.HResult);

                context.ExceptionHandled = true;
                _globalExceptionResult.Message = _webHostEnvironment.IsDevelopment()
                    ? handledException.ToString() : "服务器发生错误,请联系管理员";
                context.Result = new ObjectResult(_globalExceptionResult);
                break;
        }
        await Task.CompletedTask;
    }
}