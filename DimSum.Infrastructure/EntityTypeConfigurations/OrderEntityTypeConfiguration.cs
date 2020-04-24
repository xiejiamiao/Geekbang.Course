using System;
using System.Collections.Generic;
using System.Text;
using DimSum.Domain.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DimSum.Infrastructure.EntityTypeConfigurations
{
    public class OrderEntityTypeConfiguration:IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("order");
            builder.Property(x => x.UserId).HasMaxLength(20);
            builder.Property(x => x.UserName).HasMaxLength(30);

            builder.OwnsOne(o => o.Address, a =>
            {
                a.WithOwner();
                a.Property(x => x.City).HasMaxLength(20);
                a.Property(x => x.Street).HasMaxLength(50);
                a.Property(x => x.Zipcode).HasMaxLength(10);
            });
        }
    }
}
