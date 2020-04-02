using System;

namespace ExceptionDemo.Exceptions
{
    public class MyServerException : Exception, IKnownException
    {
        public MyServerException(int errorCode, string message, params object[] errorData) : base(message)
        {
            ErrorCode = errorCode;
            ErrorData = errorData;
        }

        public int ErrorCode { get; }
        public object[] ErrorData { get; }
    }
}
