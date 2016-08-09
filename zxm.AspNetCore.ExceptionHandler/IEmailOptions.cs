using System.Collections.Generic;
using zxm.MailKit;

namespace zxm.AspNetCore.ExceptionLogger
{
    /// <summary>
    /// Email Options
    /// </summary>
    public interface IEmailOptions
    {
        /// <summary>
        /// Email tos
        /// </summary>
        IEnumerable<MailAddress> Tos { get; set; }

        /// <summary>
        /// Email subject
        /// </summary>
        string Subject { get; set; }
    }
}
