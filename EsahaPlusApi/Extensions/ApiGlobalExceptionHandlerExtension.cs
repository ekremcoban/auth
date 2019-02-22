using Core.Core.Dtos;
using EsahaPlusApi.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;

namespace EsahaPlusApi.Extensions
{
    /*
     * Extension to catch all unhandled exceptions. 
     * */
    public static class ApiGlobalExceptionHandlerExtension
    {
        public static IApplicationBuilder UseWebApiExceptionHandler(this IApplicationBuilder app)
        {
            var loggerFactory = app.ApplicationServices.GetService(typeof(ILoggerFactory)) as ILoggerFactory;

            return app.UseExceptionHandler(HandleApiException(loggerFactory));
        }

        public static Action<IApplicationBuilder> HandleApiException(ILoggerFactory loggerFactory)
        {
            return appBuilder =>
            {
                appBuilder.Run(async context =>
                {
                    IJsonService jsonService = (IJsonService)context.RequestServices.GetService(typeof(IJsonService));

                    var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();

                    if (exceptionHandlerFeature != null)
                    {
                        var logger = loggerFactory.CreateLogger("Serilog Global exception logger");
                        logger.LogError(500, exceptionHandlerFeature.Error, exceptionHandlerFeature.Error.Message);
                    }

                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync(jsonService.SerializeObject(new ErrorDto() { Message = "Serverda hata oluştu. Teknik destek ekibi ile iletişime geçiniz." }));
                });
            };
        }
    }
}
