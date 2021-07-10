using System;
using System.Collections.Generic;
using System.Text;

namespace OVRSharp.Exceptions
{
    public class OpenVRSystemException<ErrorType> : Exception
    {
        public readonly ErrorType Error;

        public OpenVRSystemException() : base() { }
        public OpenVRSystemException(string message) : base(message) { }
        public OpenVRSystemException(string message, Exception inner) : base(message, inner) { }

        public OpenVRSystemException(string message, ErrorType error) : this(message)
        {
            Error = error;
        }
    }
}
