using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Pi.Dns.Common.Authorization
{
    public class ApiKeyAuthorizationAttribute : Attribute, IAsyncActionFilter
    {
        private const string ApiKeyHeaderName = "api_key";
        private const string ApiKeyQueryParamName = "api_key";
        private const string HostKeyConfigName = "ApiKey";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKey))
            {
                if (!context.HttpContext.Request.Query.TryGetValue(ApiKeyQueryParamName, out apiKey))
                {
                    context.Result = new UnauthorizedObjectResult("API Key not specified");
                    return;
                }
            }

            var hostKey = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>()[HostKeyConfigName];
            if (string.IsNullOrWhiteSpace(hostKey))
            {
                context.Result = new StatusCodeResult(500);
                return;
            }

            if (hostKey != apiKey)
            {
                context.Result = new UnauthorizedObjectResult("API Key invalid");
                return;
            }

            await next();
        }
    }
}
