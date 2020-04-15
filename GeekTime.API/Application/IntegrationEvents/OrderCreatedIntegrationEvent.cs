﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekTime.API.Application.IntegrationEvents
{
    public class OrderCreatedIntegrationEvent
    {
        public long OrderId { get; }

        public OrderCreatedIntegrationEvent(long orderId)
        {
            OrderId = orderId;
        }
    }
}
