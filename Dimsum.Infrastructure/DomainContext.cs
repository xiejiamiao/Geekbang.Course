using System;
using System.Collections.Generic;
using System.Text;
using Dimsum.Domain.OrderAggregate;
using Dimsum.Infrastructure.Core;
using DotNetCore.CAP;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dimsum.Infrastructure
{
    public class DomainContext:EFContext
    {
        public DomainContext(DbContextOptions options, IMediator mediator, ICapPublisher capBus) : base(options, mediator, capBus)
        {
        }

        public DbSet<Order> Orders { get; set; }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
