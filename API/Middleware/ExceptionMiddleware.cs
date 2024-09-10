
using Microsoft.Extensions.Primitives;
using Microsoft.OpenApi.Any;
using Microsoft.VisualBasic;
using System.Reflection.Metadata;
using System.Security.Authentication;
using System;
using Application.Response;
using Application.UserCQ.ViewModels;
using Domain.Enum;
using Domain.Utils;

namespace API.Middleware
{
    public class ExceptionMiddleware(RequestDelegate next)
    {

        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex) 
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task<Task> HandleExceptionAsync(HttpContext context, Exception exception)
        {

            var error = new ErrorInfo(BusinessError.InternalServerError.GetDescription(), 500);
            context.Response.StatusCode = error.HTTPStatus;
            return context.Response.WriteAsync(error.ToJson());
        }
    }
}
