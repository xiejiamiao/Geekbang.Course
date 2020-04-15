using MediatR;

namespace MediatorDemo.Commands
{
    public class MyDemoCommand:IRequest<string>
    {
        public string Data { get; }

        public MyDemoCommand(string data)
        {
            Data = data;
        }
    }
}
