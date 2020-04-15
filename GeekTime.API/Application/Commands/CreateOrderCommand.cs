using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace GeekTime.API.Application.Commands
{
    public class CreateOrderCommand : IRequest<long>
    {
    }
}
