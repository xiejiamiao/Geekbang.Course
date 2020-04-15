using System;
using System.Threading;
using System.Threading.Tasks;
using MediatorDemo.Events;
using MediatR;

namespace MediatorDemo.EventHandlers
{
    public class MyDemoEventHandler:INotificationHandler<MyDemoEvent>
    {
        public async Task Handle(MyDemoEvent notification, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            Console.WriteLine($"MyDemoEventHandler.Handle执行:{notification.EventName}");
        }
    }

    public class MyDemoEventHandlerV2 : INotificationHandler<MyDemoEvent>
    {
        public async Task Handle(MyDemoEvent notification, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            Console.WriteLine($"MyDemoEventHandlerV2.Handle执行:{notification.EventName}");
        }
    }
}
