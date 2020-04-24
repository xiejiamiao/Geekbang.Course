using System;
using System.Collections.Generic;
using System.Text;
using DimSum.Domain.OrderAggregate;
using DimSum.Domain.UserAggregate;
using DimSum.Infrastructure.Core;
using DimSum.Infrastructure.EntityTypeConfigurations;
using DotNetCore.CAP;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DimSum.Infrastructure
{
    public class DomainContext:EFContext
    {
        public DomainContext(DbContextOptions options, ICapPublisher capBus, IMediator mediator) : base(options, capBus, mediator)
        {
        }

        public DbSet<Order> Orders { get; set; }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new OrderEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
