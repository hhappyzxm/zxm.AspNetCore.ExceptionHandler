using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using Microsoft.Extensions.Options;

namespace zxm.AspNetCore.ExceptionHandler
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        
        public ExceptionHandlerMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IOptions<ExceptionHandlerOptions> options = null)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            Logger = Logger = loggerFactory.CreateLogger(this.GetType().FullName);

            Options = options.Value;
            if (Options.EmailOptions != null)
            {
                if (Options.EmailOptions.To == null)
                {
                    throw new ArgumentNullException(nameof(Options.EmailOptions.To));
                }

                if (string.IsNullOrEmpty(Options.EmailOptions.Subject))
                {
                    throw new ArgumentNullException(nameof(Options.EmailOptions.Subject));
                }

                if (Options.EmailOptions.Sender == null)
                {
                    throw new ArgumentNullException(nameof(Options.EmailOptions.Sender));
                }
            }

            _next = next;
        }

        public ExceptionHandlerOptions Options { get; set; }

        public ILogger Logger { get; set; }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                Logger.LogInformation("Has already catched exception.");

                var errMessage = BuildErrorMessage(ex, context);

                Logger.LogError(errMessage);

                if (Options.EmailOptions != null)
                {
                    Task.Run(() =>
                    {

                        try
                        {
                            Logger.LogInformation("Send error email.");
                            Options.EmailOptions.Sender.SendEmail(Options.EmailOptions.To, Options.EmailOptions.Subject, errMessage);
                            Logger.LogInformation("Send error email finished.");
                        }
                        catch (Exception emailException)
                        {
                            Logger.LogInformation("Send error email failed.");
                            Logger.LogError(BuildErrorMessage(emailException));
                        }
                    });
                }

                throw;
            }
        }
        
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
