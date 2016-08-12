using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using zxm.MailKit.Abstractions;

namespace zxm.AspNetCore.ExceptionHandler
{
    public static class ExceptionHandlerAppBuilderExtensions
    {
        public static IApplicationBuilder UseExceptionHandler(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<ExceptionHandlerMiddleware>();
        }

        public static IApplicationBuilder UseExceptionHandler(this IApplicationBuilder app, IList<MailAddress> to,
            string subject)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            var options = new ExceptionHandlerOptions {MailOptions = new MailOptions {Subject = subject, To = to}};

            return UseExceptionHandler(app, options);
        }

        public static IApplicationBuilder UseExceptionHandler(this IApplicationBuilder app, Action<Exception> manualProcess)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            var options = new ExceptionHandlerOptions { ManualProcess = manualProcess};

            return UseExceptionHandler(app, options);
        }

        public static IApplicationBuilder UseExceptionHandler(this IApplicationBuilder app, ExceptionHandlerOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            
            return app.UseMiddleware<ExceptionHandlerMiddleware>(Options.Create(options));
        }
    }
}
