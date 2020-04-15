using System.Threading;
using System.Threading.Tasks;
using MediatorDemo.Commands;
using MediatR;

namespace MediatorDemo.CommandHandlers
{
    public class MyDemoCommandHandler:IRequestHandler<MyDemoCommand,string>
    {
        public async Task<string> Handle(MyDemoCommand request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            return $"Hello from MyDemoCommandHandler.Handler -> command data = {request.Data}";
        }
    }
}
