using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcDemo.Protos;

namespace GrpcClientDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new OrderService.OrderServiceClient(channel);

            var option = int.Parse(args[0]);
            switch (option)
            {
                case 0:
                    await GetByOrderNoAsync(client);
                    break;
                case 1:
                    await GetByOwner(client);
                    break;
                case 2:
                    await AddOrder(client);
                    break;
                case 3:
                    await BatchAddOrder(client);
                    break;
            }

            Console.WriteLine("==========END==========");
        }

        public static async Task GetByOrderNoAsync(OrderService.OrderServiceClient client)
        {
            var metaData = new Metadata()
            {
                {"userName", "jiamiao.x"},
                {"clientName", "GrpcClientDemo"}
            };
            var response = await client.GetByOrderNoAsync(new GetByOrderNoRequest() {OrderNo = "2020042201"},metaData);
            Console.WriteLine($"接收到数据：{response}");
        }

        public static async Task GetByOwner(OrderService.OrderServiceClient client)
        {
            var response = client.GetByOwner(new GetByOwnerRequest() {OrderOwner = 100});
            var responseStream = response.ResponseStream;
            while (await responseStream.MoveNext())
            {
                Console.WriteLine($"接收到数据：{responseStream.Current}");
            }

            Console.WriteLine($"数据接收完毕");
        }

        public static async Task AddOrder(OrderService.OrderServiceClient client)
        {
            var order = new Order()
            {
                OrderNo = "2020042301",
                Status = 1,
                Payment = 43141.98f,
                Products =
                {
                    new Order.Types.OrderProduct()
                    {
                        ProductTitle = "OnePlus 7T",
                        SkuTitle = "256GB 蓝色",
                        Num = 1,
                        UnitPrice = 3600f
                    }
                },
                Address = new Order.Types.OrderAddress()
                {
                    Province = "广东省",
                    City = "深圳市",
                    Districe = "南山区",
                    Detail = "北科大厦7003",
                    Name = "Jiamiao.x",
                    Mobile = "13822113366"
                },
                OrderOwner = 100,
            };
            var response = await client.AddOrderAsync(order);
            Console.WriteLine($"接收到数据：{response}");
        }

        public static async Task BatchAddOrder(OrderService.OrderServiceClient client)
        {
            var orders = new List<Order>()
            {
                new Order()
                {
                    OrderNo = "2020042301",
                    Status = 1,
                    Payment = 3600f,
                    Products =
                    {
                        new Order.Types.OrderProduct()
                        {
                            ProductTitle = "OnePlus 7T",
                            SkuTitle = "256GB 蓝色",
                            Num = 1,
                            UnitPrice = 3600f
                        }
                    },
                    Address = new Order.Types.OrderAddress()
                    {
                        Province = "广东省",
                        City = "深圳市",
                        Districe = "南山区",
                        Detail = "北科大厦7003",
                        Name = "Jiamiao.x",
                        Mobile = "13822113366"
                    },
                    OrderOwner = 100,
                },
                new Order()
                {
                    OrderNo = "2020042302",
                    Status = 1,
                    Payment = 13999.99f,
                    Products =
                    {
                        new Order.Types.OrderProduct()
                        {
                            ProductTitle = "SONY PS4 Pro",
                            SkuTitle = "1TB 黑色",
                            Num = 1,
                            UnitPrice = 3999.99f
                        },
                        new Order.Types.OrderProduct()
                        {
                            ProductTitle = "Surface Desktop Pro",
                            SkuTitle = "1TB 白色",
                            Num = 1,
                            UnitPrice = 13999.99f
                        }
                    },
                    Address = new Order.Types.OrderAddress()
                    {
                        Province = "广东省",
                        City = "深圳市",
                        Districe = "南山区",
                        Detail = "北科大厦7003",
                        Name = "Jiamiao.x",
                        Mobile = "13822113366"
                    },
                    OrderOwner = 100,
                }
            };
            var call = client.BatchAddOrder();
            
            foreach (var order in orders)
            {
                await call.RequestStream.WriteAsync(order);
            }

            await call.RequestStream.CompleteAsync();
            Console.WriteLine("----数据发送完毕----");
            await Task.Run(async () =>
            {
                while (await call.ResponseStream.MoveNext())
                {
                    Console.WriteLine($"接收到消息：{call.ResponseStream.Current}");
                }
            });
        }
    }
}
