using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using GrpcDemo.Protos;
using GrpcServerDemo.Data;
using Microsoft.Extensions.Logging;

namespace GrpcServerDemo.Services
{
    public class DemoOrderService : OrderService.OrderServiceBase
    {
        private readonly ILogger<DemoOrderService> _logger;

        public DemoOrderService(ILogger<DemoOrderService> logger)
        {
            _logger = logger;
        }


        public override async Task<Order> GetByOrderNo(GetByOrderNoRequest request, ServerCallContext context)
        {
            _logger.LogInformation("有人请求接口 -> GetByOrderNo");
            var metaData = context.RequestHeaders;
            foreach (var item in metaData)
            {
                _logger.LogInformation($"{item.Key}: {item.Value}");
            }
            await Task.CompletedTask;
            var dbValue = InMemoryData.Orders.FirstOrDefault(x => x.OrderNo == request.OrderNo);
            if (dbValue != null)
            {
                return dbValue;
            }
            else
            {
                throw  new Exception("订单号错误");
            }
        }

        public override async Task GetByOwner(GetByOwnerRequest request, IServerStreamWriter<Order> responseStream, ServerCallContext context)
        {
            _logger.LogInformation("有人请求接口 -> GetByOwner");
            var dbValue = InMemoryData.Orders.Where(x => x.OrderOwner == request.OrderOwner);
            foreach (var item in dbValue)
            {
                Thread.Sleep(2000);
                _logger.LogInformation($"发送数据：{item}");
                await responseStream.WriteAsync(item);
            }
        }

        public override async Task<Order> AddOrder(Order request, ServerCallContext context)
        {
            _logger.LogInformation("有人请求接口 -> AddOrder");
            await Task.CompletedTask;
            request.Id = InMemoryData.Orders.Max(x => x.Id) + 1;
            InMemoryData.Orders.Add(request);
            return request;
        }

        public override async Task BatchAddOrder(IAsyncStreamReader<Order> requestStream, IServerStreamWriter<Order> responseStream, ServerCallContext context)
        {
            _logger.LogInformation("有人请求接口 -> BatchAddOrder");

            while (await requestStream.MoveNext())
            {
                var inputOrder = requestStream.Current;
                lock (this)
                {
                    _logger.LogInformation($"接受数据：{inputOrder}");
                    inputOrder.Id = InMemoryData.Orders.Max(x => x.Id) + 1;
                    InMemoryData.Orders.Add(inputOrder);
                }
                await responseStream.WriteAsync(inputOrder);
                Thread.Sleep(5000);
            }
        }
    }
}
