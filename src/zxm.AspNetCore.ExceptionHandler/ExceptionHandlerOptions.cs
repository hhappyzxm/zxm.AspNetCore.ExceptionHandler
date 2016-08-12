using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using zxm.MailKit;
using zxm.MailKit.Abstractions;

namespace zxm.AspNetCore.ExceptionHandler
{
    /// <summary>
    /// EmailOptions
    /// </summary>
    public class ExceptionHandlerOptions
    {
        public MailOptions MailOptions { get; set; }

        public Action<Exception> ManualProcess { get; set; }
    }
}
