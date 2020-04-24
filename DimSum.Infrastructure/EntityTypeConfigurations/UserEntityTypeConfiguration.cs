using System;
using System.Collections.Generic;
using System.Text;
using DimSum.Domain.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DimSum.Infrastructure.EntityTypeConfigurations
{
    public class UserEntityTypeConfiguration:IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("user");
        }
    }
}
