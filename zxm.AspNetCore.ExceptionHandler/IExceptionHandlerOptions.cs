using System.Collections.Generic;
using zxm.MailKit;

namespace zxm.AspNetCore.ExceptionHandler
{
    /// <summary>
    /// Email Options
    /// </summary>
    public interface IExceptionHandlerOptions
    {
        /// <summary>
        /// Email tos
        /// </summary>
        IEnumerable<MailAddress> To { get; set; }

        /// <summary>
        /// Email subject
        /// </summary>
        string Subject { get; set; }
    }
}
