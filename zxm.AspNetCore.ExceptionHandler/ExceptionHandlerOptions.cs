using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using zxm.MailKit;

namespace zxm.AspNetCore.ExceptionHandler
{
    /// <summary>
    /// EmailOptions
    /// </summary>
    public class ExceptionHandlerOptions : IExceptionHandlerOptions
    {
        /// <summary>
        /// Constructor of EmailOptions
        /// </summary>
        /// <param name="tos"></param>
        /// <param name="subject"></param>
        public ExceptionHandlerOptions(IEnumerable<MailAddress> to, string subject)
        {
            if (to == null)
            {
                throw new ArgumentNullException(nameof(to));
            }

            if (string.IsNullOrEmpty(subject))
            {
                throw new ArgumentNullException(nameof(subject));
            }

            To = to;
            Subject = subject;
        }

        /// <summary>
        /// Email Tos
        /// </summary>
        public IEnumerable<MailAddress> To { get; set; }


        /// <summary>
        /// Email subject
        /// </summary>
        public string Subject { get; set; }
    }
}
