using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroAC.Core.Exceptions
{
    public class AuthenticationFailedException : Exception
    {
        public AuthenticationFailedException() { }

        public AuthenticationFailedException(string message) 
            : base(message) { }

        public AuthenticationFailedException(string message, Exception inner) 
            : base(message, inner) { }
    }
}
