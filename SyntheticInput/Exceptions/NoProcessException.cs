using System;
using System.Collections.Generic;
using System.Text;

namespace SyntheticInput.Exceptions
{
    public class NoProcessException : Exception
    {
        public NoProcessException()
        {
        }

        public NoProcessException(string message) : base(message)
        {
        }

        public NoProcessException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
