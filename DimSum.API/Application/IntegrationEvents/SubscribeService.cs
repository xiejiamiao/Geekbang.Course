using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCore.CAP;
using MediatR;

namespace DimSum.API.Application.IntegrationEvents
{
    public class SubscribeService : ISubscribeService, ICapSubscribe
    {
        private readonly IMediator _mediator;

        public SubscribeService(IMediator mediator)
        {
            _mediator = mediator;
        }
    }
}
