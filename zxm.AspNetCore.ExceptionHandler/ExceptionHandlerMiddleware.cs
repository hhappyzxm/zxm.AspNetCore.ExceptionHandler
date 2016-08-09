using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using zxm.MailKit;

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
        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger, IEmailOptions emailOptions = null, IMailSender mailSender = null)
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
                // Build error message body
                var sb = new StringBuilder();
                sb.AppendLine("");
                BuildErrorMessage(ex, sb);
                var errorMessageBody = sb.ToString();

                // Log error message
                _logger.LogError(errorMessageBody);

                // Send error email
                if (_mailSender != null && _emailOptions != null)
                {
                    await
                            _mailSender.SendEmailAsync(_emailOptions.Tos,
                                _emailOptions.Subject,
                                errorMessageBody);
                }

                throw;
            }
        }

        /// <summary>
        /// Build error message includes inner exception messages
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="sb"></param>
        private void BuildErrorMessage(Exception ex, StringBuilder sb)
        {
            sb.AppendLine($"Message: {ex.Message}");
            sb.AppendLine($"Source: {ex.Source}");
            sb.AppendLine($"StackTrace: {ex.StackTrace}");

            if (ex.InnerException != null)
            {
                sb.AppendLine("-------------------- InnertException --------------------");
                BuildErrorMessage(ex.InnerException, sb);
            }
        }
    }
}
