using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNetCore.CAP;
using GeekTime.Domain.OrderAggregate;
using GeekTime.Infrastructure.Repositories;
using MediatR;

namespace GeekTime.API.Application.Commands
{
    public class CreateOrderCommandHandler:IRequestHandler<CreateOrderCommand,long>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICapPublisher _capPublisher;

        public CreateOrderCommandHandler(IOrderRepository orderRepository, ICapPublisher capPublisher)
        {
            _orderRepository = orderRepository;
            _capPublisher = capPublisher;
        }

        public async Task<long> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var address = new Address("南海大道","深圳","518000");
            var order = new Order("Jiamiao.x", "谢佳淼", 25, address);

            await _orderRepository.AddAsync(order, cancellationToken);
            await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            return order.Id;
        }
    }
}
