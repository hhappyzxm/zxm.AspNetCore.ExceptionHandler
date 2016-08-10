using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using zxm.MailKit;

namespace zxm.AspNetCore.ExceptionHandler
{
    public static class ExceptionHandler
    {
        /// <summary>
        /// Use ExceptionHandlerMiddleware
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlerMiddleware>();
        }

        /// <summary>
        /// Allow send email when catch exception
        /// </summary>
        /// <param name="services"></param>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="mailSenderFunc"></param>
        /// <returns></returns>
        public static IServiceCollection AddExceptionHandler(this IServiceCollection services, IList<MailAddress> to, string subject, Func<IMailSender> mailSenderFunc)
        {
            if (mailSenderFunc == null)
            {
                throw new ArgumentNullException(nameof(mailSenderFunc));
            }

            services.AddSingleton(provider => new ExceptionHandlerOptions(to, subject));
            services.AddSingleton(provider => mailSenderFunc());

            return services;
        }
    }
}
