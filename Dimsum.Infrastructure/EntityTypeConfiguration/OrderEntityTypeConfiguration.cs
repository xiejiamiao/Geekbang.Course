using System;
using System.Collections.Generic;
using System.Text;
using Dimsum.Domain.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dimsum.Infrastructure.EntityTypeConfiguration
{
    class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("order");
            builder.OwnsOne(o => o.Address, a =>
            {
                a.WithOwner();
                a.Property(x => x.Name).HasMaxLength(10);
                a.Property(x => x.Detail).HasMaxLength(200);
                a.Property(x => x.Mobile).HasMaxLength(15);
                a.Property(x => x.Province).HasMaxLength(10);
                a.Property(x => x.City).HasMaxLength(20);
                a.Property(x => x.District).HasMaxLength(20);
                a.Property(x => x.Zipcode).HasMaxLength(10);
            });
            builder.Property(x => x.OrderNo).HasMaxLength(20);
            builder.Property(x => x.UserId).HasMaxLength(20);
        }
    }
}
