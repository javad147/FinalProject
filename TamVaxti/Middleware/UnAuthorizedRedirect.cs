using System;
namespace TamVaxti.Middleware
{

    public class UnAuthorizedRedirect
    {
        private readonly RequestDelegate _next;

        public UnAuthorizedRedirect(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
            {
                var path = context.Request.Path.Value;

                if (path.StartsWith("/Admin", StringComparison.OrdinalIgnoreCase))
                {
                    context.Response.Redirect("/Admin/Dashboard/AccessDenied");
                }
                else
                {
                    context.Response.Redirect("/Account/AccessDenied");
                }
            }
        }
    }
}

