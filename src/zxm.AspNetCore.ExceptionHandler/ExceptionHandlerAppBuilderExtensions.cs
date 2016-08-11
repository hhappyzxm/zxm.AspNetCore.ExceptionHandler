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
        /// <summary>
        /// Use ExceptionHandlerMiddleware
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseExceptionHandler(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<ExceptionHandlerMiddleware>();
        }

        public static IApplicationBuilder UseExceptionHandler(this IApplicationBuilder app, IList<MailAddress> to, string subject)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            var options = new ExceptionHandlerOptions { To = to, Subject = subject };
            return app.UseMiddleware<ExceptionHandlerMiddleware>(Options.Create(options));
        }

        /// <summary>
        /// Allow send email when catch exception
        /// </summary>
        /// <param name="services"></param>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="mailSenderFunc"></param>
        /// <returns></returns>
        public static IServiceCollection AddExceptionHandler(this IServiceCollection services, string subject, Func<IMailSender> mailSenderFunc)
        {
            if (mailSenderFunc == null)
            {
                throw new ArgumentNullException(nameof(mailSenderFunc));
            }

            services.AddSingleton(provider => new ExceptionHandlerOptions { To = to, Subject = subject});
            services.AddSingleton(provider => mailSenderFunc());

            return services;
        }
    }
}
