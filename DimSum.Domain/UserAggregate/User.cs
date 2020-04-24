using System;
using System.Collections.Generic;
using System.Text;
using DimSum.Domain.Abstractions;

namespace DimSum.Domain.UserAggregate
{
    public class User : Entity<long>, IAggregateRoot
    {

    }
}
