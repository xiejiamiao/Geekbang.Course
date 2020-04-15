using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace GeekTime.API.Application.Queries
{
    public class MyOrderQueryHandler:IRequestHandler<MyOrderQuery,List<string>>
    {
        public async Task<List<string>> Handle(MyOrderQuery request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            return new List<string>();
        }
    }
}
