using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeekTime.API.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GeekTime.API.Controllers
{
    [ApiController]
    public class OrderController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IMediator mediator,ILogger<OrderController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        public async Task<List<string>> QueryOrder([FromBody] MyOrderQuery myOrderQuery)
        {
            return await _mediator.Send(myOrderQuery);
        }
    }
}