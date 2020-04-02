namespace ExceptionDemo.Exceptions
{
    public class KnownException:IKnownException
    {
        public KnownException(object[] errorData, int errorCode, string message)
        {
            ErrorData = errorData;
            ErrorCode = errorCode;
            Message = message;
        }

        public string Message { get; }
        public int ErrorCode { get; }
        public object[] ErrorData { get; }

        public static readonly IKnownException UnKnown = new KnownException(errorData: new object[] { }, errorCode: 9999, message: "未知错误");

        public static IKnownException FromKnownException(IKnownException exception)
        {
            return new KnownException(errorData: exception.ErrorData, errorCode: exception.ErrorCode, message: exception.Message);
        }
    }
}