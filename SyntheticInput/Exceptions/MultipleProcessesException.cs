using System;
using System.Collections.Generic;
using System.Text;

namespace SyntheticInput.Exceptions
{
    public class MultipleProcessesException : Exception
    {
        public MultipleProcessesException()
        {
        }

        public MultipleProcessesException(string message) : base(message)
        {
        }

        public MultipleProcessesException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
