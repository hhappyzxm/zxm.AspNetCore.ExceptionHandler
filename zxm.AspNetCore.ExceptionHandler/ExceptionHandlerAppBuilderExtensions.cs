using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using zxm.MailKit;
using Microsoft.Extensions.Options;

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

        public static IApplicationBuilder UseExceptionHandler(this IApplicationBuilder app, IList<MailAddress> to, string subject, IMailSender mailSender)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            var options = new ExceptionHandlerOptions
            {
                EmailOptions = new EmailOptions
                {
                    To = to,
                    Subject = subject,
                    Sender = mailSender
                }
            };
            return app.UseMiddleware<ExceptionHandlerMiddleware>(Options.Create(options));
        }
    }
}
