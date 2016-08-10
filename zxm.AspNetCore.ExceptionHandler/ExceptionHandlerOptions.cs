using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using zxm.MailKit;

namespace zxm.AspNetCore.ExceptionHandler
{
    public class ExceptionHandlerOptions
    {
        public EmailOptions EmailOptions { get; set; }
    }
}
