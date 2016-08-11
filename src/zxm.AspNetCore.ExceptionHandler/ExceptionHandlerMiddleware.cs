using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using zxm.MailKit;
using Newtonsoft.Json;
using System.IO;
using Microsoft.Extensions.Options;

namespace zxm.AspNetCore.ExceptionHandler
{
    /// <summary>
    /// ExceptionLoggerMiddleware
    /// </summary>
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Constructor of ExceptionLoggerMiddleware
        /// </summary>
        /// <param name="next"></param>
        /// <param name="logger"></param>
        /// <param name="emailOptions"></param>
        /// <param name="emailSender"></param>
        public ExceptionHandlerMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IOptions<ExceptionHandlerOptions> options = null, IMailSender mailSender = null)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            Logger = Logger = loggerFactory.CreateLogger(this.GetType().FullName);

            Options = options.Value;
            if (Options != null)
            {
                if (Options.To == null)
                {
                    throw new ArgumentNullException(nameof(Options.To));
                }

                if (string.IsNullOrEmpty(Options.Subject))
                {
                    throw new ArgumentNullException(nameof(Options.Subject));
                }

            }

            MailSender = mailSender;

            _next = next;
        }

        public ExceptionHandlerOptions Options { get; set; }

        public ILogger Logger { get; set; }

        public IMailSender MailSender { get; set; }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                var errMessage = BuildErrorMessage(ex, context);

                LogError(errMessage);
                
                if (MailSender != null && Options != null)
                {
                    Task.Run(() =>
                    {
                        try
                        {
                            MailSender.SendEmail(Options.To, Options.Subject, errMessage);
                        }
                        catch (Exception emailException)
                        {
                            LogError(BuildErrorMessage(emailException));
                        }
                    });
                }

                throw;
            }
        }

        /// <summary>
        /// Log error if _logger is not null
        /// </summary>
        /// <param name="message"></param>
        private void LogError(string message)
        {
            Logger.LogError(message);
        }

        /// <summary>
        /// Build error message
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private string BuildErrorMessage(Exception ex, HttpContext context = null)
        {
            var sb = new StringBuilder();
            sb.AppendLine("");
            GetErrorMessage(ex, sb);
            if (context != null)
            {
                sb.AppendLine("-------------------- Request Infomation --------------------");
                GetRequestInfo(context, sb);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Get request info
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sb"></param>
        private void GetRequestInfo(HttpContext context, StringBuilder sb)
        {
            sb.AppendLine($"Request Head: {JsonConvert.SerializeObject(context.Request.Headers)}");
            sb.AppendLine($"Request Host: {context.Request.Host}");
            sb.AppendLine($"Request Path: {context.Request.Path}");
            sb.AppendLine($"Request Query String: {context.Request.QueryString}");

            var bodyString = string.Empty;
            using (var stremReader = new StreamReader(context.Request.Body, Encoding.UTF8))
            {
                bodyString = stremReader.ReadToEnd();
            }
            sb.AppendLine($"Request Body: {bodyString}");
        }

        /// <summary>
        /// Get error message includes inner exception messages
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="sb"></param>
        private void GetErrorMessage(Exception ex, StringBuilder sb)
        {
            sb.AppendLine($"Message: {ex.Message}");
            sb.AppendLine($"Source: {ex.Source}");
            sb.AppendLine($"StackTrace:");
            sb.AppendLine(ex.StackTrace);

            if (ex.InnerException != null)
            {
                sb.AppendLine("-------------------- InnertException --------------------");
                GetErrorMessage(ex.InnerException, sb);
            }
        }
    }
}
