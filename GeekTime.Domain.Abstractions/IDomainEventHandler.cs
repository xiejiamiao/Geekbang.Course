using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace GeekTime.Domain.Abstractions
{
    public interface IDomainEventHandler<TDomainEvent>:INotificationHandler<TDomainEvent> where TDomainEvent:IDomainEvent
    {
        //这里我们使用INotificationHandler的Handler方法来作为处理方法的定义
        // Task Handle(TDomainEvent domainEvent,CancellationToken cancellationToken);
    }
}
