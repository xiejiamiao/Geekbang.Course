using System;
using System.Collections.Generic;
using System.Text;
using DotNetCore.CAP;
using GeekTime.Domain.OrderAggregate;
using GeekTime.Domain.UserAggregate;
using GeekTime.Infrastructure.Core;
using GeekTime.Infrastructure.EntityConfigurations;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GeekTime.Infrastructure
{
    public class DomainContext:EFContext
    {
        public DomainContext(DbContextOptions options, IMediator mediator, ICapPublisher capPublisher) : base(options,
            mediator, capPublisher)
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
