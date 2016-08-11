using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using zxm.MailKit;
using zxm.MailKit.Abstractions;

namespace zxm.AspNetCore.ExceptionHandler
{
    public class MailOptions
    {
        public string Subject { get; set; }

        public IEnumerable<MailAddress> To { get; set; }
    }
}
