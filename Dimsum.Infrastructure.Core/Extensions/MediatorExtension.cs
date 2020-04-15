using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dimsum.Domain.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dimsum.Infrastructure.Core.Extensions
{
    public static class MediatorExtension
    {
        public static async Task DispatchDomainEventAsync(this IMediator mediator, DbContext ctx)
        {
            var domainEntities = ctx.ChangeTracker.Entries<Entity>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());

            var domainEvents = domainEntities.SelectMany(x => x.Entity.DomainEvents).ToList();

            domainEntities.ToList().ForEach(entity => entity.Entity.ClearDomainEvent());
            domainEvents.ForEach(async @event => await mediator.Publish(@event));
        }
    }
}
