using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DimSum.Domain.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DimSum.Infrastructure.Core.Extensions
{
    internal static class MediatorExtension
    {
        public static async Task DispatchDomainEventAsync(this IMediator mediator, DbContext ctx)
        {
            var domainEntities = ctx.ChangeTracker.Entries<Entity>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());

            var domainEvents = domainEntities.SelectMany(x => x.Entity.DomainEvents).ToList();

            domainEvents.ForEach(async x => await mediator.Publish(x));
            domainEntities.ToList().ForEach(x=>x.Entity.ClearDomainEvent());
        }
    }
}
