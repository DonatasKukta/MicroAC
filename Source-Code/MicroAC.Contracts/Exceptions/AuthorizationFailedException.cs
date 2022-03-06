using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroAC.Core.Exceptions
{
    public class AuthorizationFailedException : Exception
    {
        public AuthorizationFailedException() { }

        public AuthorizationFailedException(string message) 
            : base(message) { }

        public AuthorizationFailedException(string message, Exception inner) 
            : base(message, inner) { }
    }
}
