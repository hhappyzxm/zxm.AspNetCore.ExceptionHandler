using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using zxm.MailKit;
using Newtonsoft.Json;
using System.IO;

namespace zxm.AspNetCore.ExceptionLogger
{
    /// <summary>
    /// ExceptionLoggerMiddleware
    /// </summary>
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly IMailSender _mailSender;
        private readonly IEmailOptions _emailOptions;

        /// <summary>
        /// Constructor of ExceptionLoggerMiddleware
        /// </summary>
        /// <param name="next"></param>
        /// <param name="logger"></param>
        /// <param name="emailOptions"></param>
        /// <param name="emailSender"></param>
        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger = null, IEmailOptions emailOptions = null, IMailSender mailSender = null)
        {
            _next = next;
            _logger = logger;
            _mailSender = mailSender;
            _emailOptions = emailOptions;
        }

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
                
                if (_mailSender != null && _emailOptions != null)
                {
                    Task.Run(() =>
                    {
                        try
                        {
                            _mailSender.SendEmail(_emailOptions.To, _emailOptions.Subject, errMessage);
                        }
                        catch (Exception emailException)
                        {
                            LogError(BuildErrorMessage(emailException, context));
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
            if (_logger != null)
            {
                _logger.LogError(message);
            }
        }

        /// <summary>
        /// Build error message
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private string BuildErrorMessage(Exception ex, HttpContext context)
        {
            var sb = new StringBuilder();
            sb.AppendLine("");
            GetRequestInfo(context, sb);
            sb.AppendLine("");
            GetErrorMessage(ex, sb);
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
