using System;

namespace ExceptionDemo.Exceptions
{
    public class InvalidParameterException: Exception,IKnownException
    {
        public InvalidParameterException(int errorCode, string message, params object[] errorData) : base(message)
        {
            ErrorCode = errorCode;
            ErrorData = errorData;
        }

        public int ErrorCode { get; }
        public object[] ErrorData { get; }
    }
}
