using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Diagnostics;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IOptionsSnapshot<GlobalResult> options,
    IWebHostEnvironment webHostEnvironment) : IExceptionHandler
{
    private readonly GlobalResult _globalResult = options.Get("Global_Exception");

    private readonly GlobalResult _businessResult = options.Get("Business_Exception");

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        switch (exception)
        {
            case BizException businessException:
                logger.LogError(businessException, "Title:业务异常 HResult:{HResult}", businessException.HResult);

                _businessResult.Message = businessException.Message;
                await context.Response.WriteAsJsonAsync(_businessResult, cancellationToken);
                return await ValueTask.FromResult(true);
            case Exception handledException:
                logger.LogError(handledException, "Title:系统异常 HResult:{HResult}", handledException.HResult);

                _globalResult.Message = webHostEnvironment.IsDevelopment()
                    ? handledException.ToString() : "服务器发生错误,请联系管理员";
                await context.Response.WriteAsJsonAsync(_globalResult, cancellationToken);
                return await ValueTask.FromResult(true);
        }
        return await ValueTask.FromResult(false);
    }
}