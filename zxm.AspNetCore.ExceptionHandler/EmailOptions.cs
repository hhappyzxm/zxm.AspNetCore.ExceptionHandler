using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using zxm.MailKit;

namespace zxm.AspNetCore.ExceptionHandler
{
    public class EmailOptions
    {
        public IEnumerable<MailAddress> To { get; set; }

        public string Subject { get; set; }

        public IMailSender Sender { get; set; }
    }
}
