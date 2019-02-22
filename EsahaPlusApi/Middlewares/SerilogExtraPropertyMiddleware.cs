using Microsoft.AspNetCore.Http;
using Serilog.Context;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EsahaPlusApi.Middlewares
{
    /*
     * Adding extra Username property into log info
     * */
    public class SerilogExtraPropertyMiddleware
    {
        private readonly RequestDelegate next;

        public SerilogExtraPropertyMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public Task Invoke(HttpContext context)
        {
            LogContext.PushProperty("UserId", context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            LogContext.PushProperty("Username", context.User.FindFirst(ClaimTypes.Name).Value);

            return next(context);
        }
    }
}
