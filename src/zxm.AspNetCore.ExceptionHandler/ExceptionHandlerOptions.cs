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
        /// Email Tos
        /// </summary>
        public IEnumerable<MailAddress> To { get; set; }


        /// <summary>
        /// Email subject
        /// </summary>
        public string Subject { get; set; }
    }
}
