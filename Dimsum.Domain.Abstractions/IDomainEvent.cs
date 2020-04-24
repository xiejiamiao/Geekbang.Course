using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace DimSum.Domain.Abstractions
{
    public interface IDomainEvent : INotification
    {
    }
}
