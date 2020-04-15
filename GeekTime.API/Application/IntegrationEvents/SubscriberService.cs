using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCore.CAP;
using MediatR;

namespace GeekTime.API.Application.IntegrationEvents
{
    public class SubscriberService : ISubscriberService, ICapSubscribe
    {
        private readonly IMediator _mediator;

        public SubscriberService(IMediator mediator)
        {
            _mediator = mediator;
        }

        [CapSubscribe("OrderCreated")]
        public void OrderCreated(OrderCreatedIntegrationEvent @event)
        {
            //Do Something
        }
    }
}
