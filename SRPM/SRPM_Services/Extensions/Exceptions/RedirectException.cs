using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.Extensions.Exceptions
{
    public class RedirectException : Exception
    {
        public string RedirectUrl { get; }

        public RedirectException(string redirectUrl, string message = null!) : base(message)
        {
            RedirectUrl = redirectUrl;
        }
    }

}
