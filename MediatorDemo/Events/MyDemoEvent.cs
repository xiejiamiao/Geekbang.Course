using MediatR;

namespace MediatorDemo.Events
{
    public class MyDemoEvent:INotification
    {
        public string EventName { get; }

        public MyDemoEvent(string eventName)
        {
            EventName = eventName;
        }
    }
}
