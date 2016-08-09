using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using zxm.MailKit;

namespace zxm.AspNetCore.ExceptionLogger
{
    /// <summary>
    /// EmailOptions
    /// </summary>
    public class EmailOptions : IEmailOptions
    {
        /// <summary>
        /// Constructor of EmailOptions
        /// </summary>
        /// <param name="tos"></param>
        /// <param name="subject"></param>
        public EmailOptions(IEnumerable<MailAddress> tos, string subject)
        {
            if (tos == null)
            {
                throw new ArgumentNullException(nameof(tos));
            }

            if (string.IsNullOrEmpty(subject))
            {
                throw new ArgumentNullException(nameof(subject));
            }

            To = tos;
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
