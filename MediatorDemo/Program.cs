using System;
using System.Threading.Tasks;
using MediatorDemo.Commands;
using MediatorDemo.Events;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace MediatorDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var service = new ServiceCollection();
            service.AddMediatR(typeof(Program).Assembly);

            var serviceProvider = service.BuildServiceProvider();
            var mediator = serviceProvider.GetService<IMediator>();

            var rsp = await mediator.Send(new MyDemoCommand("This is my demo command"));
            Console.WriteLine(rsp);
            Console.WriteLine("==========");

            await mediator.Publish(new MyDemoEvent("MyEvent"));

            Console.ReadKey();
        }
    }
}
