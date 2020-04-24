using System;
using System.Collections.Generic;
using System.Text;

namespace DimSum.Core
{
    public interface IKnownException
    {
        string Message { get; }

        int ErrorCode { get; }

        object[] ErrorDate { get; }
    }
}
