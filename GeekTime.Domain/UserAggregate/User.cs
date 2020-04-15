using System;
using System.Collections.Generic;
using System.Text;
using GeekTime.Domain.Abstractions;

namespace GeekTime.Domain.UserAggregate
{
    public class User:Entity<long>,IAggregateRoot
    {
    }
}
