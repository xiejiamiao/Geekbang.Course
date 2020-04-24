using System;
using System.Collections.Generic;
using System.Text;

namespace DimSum.Core
{
    public class KnownException:IKnownException
    {
        public KnownException(string message, int errorCode, object[] errorDate)
        {
            Message = message;
            ErrorCode = errorCode;
            ErrorDate = errorDate;
        }

        public string Message { get; }
        public int ErrorCode { get; }
        public object[] ErrorDate { get; }

        public static readonly IKnownException Unknown = new KnownException(message: "未知错误", errorCode: 9999, errorDate: new object[] { });

        public static IKnownException FromKnownException(IKnownException exception)
        {
            return new KnownException(message: exception.Message, errorCode: exception.ErrorCode,
                errorDate: exception.ErrorDate);
        }
    }
}
