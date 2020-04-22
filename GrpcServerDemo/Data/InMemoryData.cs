using System.Collections.Generic;
using GrpcDemo.Protos;

namespace GrpcServerDemo.Data
{
    public class InMemoryData
    {
        public static List<Order> Orders = new List<Order>()
        {
            new Order()
            {
                Id = 1,
                OrderNo = "2020042201",
                Status = 1,
                Payment = 43141.98f,
                Products =
                {
                    new Order.Types.OrderProduct()
                    {
                        ProductTitle = "Apple iPhone11",
                        SkuTitle = "256GB 黑色",
                        Num = 2,
                        UnitPrice = 9999.99f
                    },
                    new Order.Types.OrderProduct()
                    {
                        ProductTitle = "Apple MacBook Pro",
                        SkuTitle = "i7 512GB 灰色",
                        Num = 1,
                        UnitPrice = 23142
                    }
                },
                Address = new Order.Types.OrderAddress()
                {
                    Province = "广东省",
                    City = "深圳市",
                    Districe = "南山区",
                    Detail = "Nanshan Road 1234",
                    Name = "Jiamiao.x",
                    Mobile = "13500000000"
                },
                OrderOwner = 100,
            },
            new Order()
            {
                Id = 2,
                OrderNo = "2020042202",
                Status = 2,
                Payment = 56.00f,
                Products =
                {
                    new Order.Types.OrderProduct()
                    {
                        ProductTitle = "ASP.NET Core微服务实战",
                        SkuTitle = "1本",
                        Num = 1,
                        UnitPrice = 56.00f
                    }
                },
                Address = new Order.Types.OrderAddress()
                {
                    Province = "广东省",
                    City = "深圳市",
                    Districe = "南山区",
                    Detail = "Nanshan Road 1234",
                    Name = "Jiamiao.x",
                    Mobile = "13500000000"
                },
                OrderOwner = 100
            }
        };
    }
}
