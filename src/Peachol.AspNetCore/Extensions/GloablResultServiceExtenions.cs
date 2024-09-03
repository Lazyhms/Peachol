using Microsoft.AspNetCore.Mvc;

namespace Microsoft.Extensions.DependencyInjection;

public static class GloablResultServiceExtenions
{
    /// <summary>
    /// Configure the global return <see cref="GlobalResult"/> format
    /// </summary>
    public static IServiceCollection ConfigureGlobalResult(this IServiceCollection services)
        => services
            .ConfigureSuccessResult(options => { options.Code = 0; options.Message = "操作成功."; })
            .ConfigureBusinessException(options => { options.Code = 1; options.Message = "业务异常."; })
            .ConfigureGlobalException(options => { options.Code = 2; options.Message = "服务器异常."; });

    /// <summary>
    /// This should be used when no exception is thrown.
    /// </summary>
    public static IServiceCollection ConfigureSuccessResult(this IServiceCollection services, Action<GlobalObjectResult> configureOptions)
        => services.Configure<GlobalObjectResult>("Global_Success", options => configureOptions(options));

    /// <summary>
    /// This should be used when a <see cref="BusinessException"/> is thrown.
    /// </summary>
    public static IServiceCollection ConfigureBusinessException(this IServiceCollection services, Action<GlobalResult> configureOptions)
        => services.Configure<GlobalResult>("Business_Exception", options => configureOptions(options));

    /// <summary>
    /// This should be used when an <see cref="Exception"/> is thrown.
    /// </summary>
    public static IServiceCollection ConfigureGlobalException(this IServiceCollection services, Action<GlobalResult> configureOptions)
        => services.Configure<GlobalResult>("Global_Exception", options => configureOptions(options));
}