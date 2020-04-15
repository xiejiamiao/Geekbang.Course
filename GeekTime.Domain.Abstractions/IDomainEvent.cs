using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace GeekTime.Domain.Abstractions
{
    public interface IDomainEvent : INotification
    {
    }
}
