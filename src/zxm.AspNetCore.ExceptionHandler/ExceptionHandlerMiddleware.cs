﻿using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using zxm.MailKit;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Options;
using zxm.MailKit.Abstractions;

namespace zxm.AspNetCore.ExceptionHandler
{
    /// <summary>
    /// ExceptionLoggerMiddleware
    /// </summary>
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        
        public ExceptionHandlerMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IOptions<ExceptionHandlerOptions> options = null, IMailSender mailSender = null)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            Logger = loggerFactory.CreateLogger(this.GetType().FullName);

            if (options != null)
            {
                Options = options.Value;
                if (Options.MailOptions != null)
                {
                    if (Options.MailOptions.To == null)
                    {
                        throw new ArgumentNullException(nameof(Options.MailOptions.To));
                    }

                    if (Options.MailOptions.To.Count() == 0)
                    {
                        throw new Exception("At lease has one email to address.");
                    }

                    if (string.IsNullOrEmpty(Options.MailOptions.Subject))
                    {
                        throw new ArgumentNullException(nameof(Options.MailOptions.Subject));
                    }

                    if (mailSender == null)
                    {
                        throw new ArgumentNullException(nameof(mailSender));
                    }

                    MailSender = mailSender;
                }
            }

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
                Logger.LogError(0, ex, "An unhandled exception has occurred: " + ex.Message);

                var errMessage = BuildErrorMessage(ex, context);
                Logger.LogError(errMessage);

                if (Options?.MailOptions != null && MailSender != null)
                {
                    try
                    {
                        await MailSender.SendEmailAsync(Options.MailOptions.To, Options.MailOptions.Subject, errMessage);
                    }
                    catch (Exception ex2)
                    {
                        Logger.LogError(0, ex2, "An unhandled exception has occurred during send error email: " + ex2.Message);
                        Logger.LogError(BuildErrorMessage(ex2));
                    }
                }

                throw;
            }
        }

        private string BuildErrorMessage(Exception ex, HttpContext context = null)
        {
            var sb = new StringBuilder();
            sb.AppendLine("-------------------- Exception Details --------------------");
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
