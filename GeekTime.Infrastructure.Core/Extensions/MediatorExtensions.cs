using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeekTime.Domain.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GeekTime.Infrastructure.Core.Extensions
{
    public static class MediatorExtensions
    {
        public static async Task DispatchDomainEventAsync(this IMediator mediator, DbContext ctx)
        {
            var domainEntities = ctx.ChangeTracker.Entries<Entity>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any()).ToList();

            var domainEvent = domainEntities.SelectMany(x => x.Entity.DomainEvents).ToList();

            domainEntities.ForEach(x=>x.Entity.ClearDomainEvents());

            foreach (var @event in domainEvent)
            {
                await mediator.Publish(@event);
            }
        }
    }
}
