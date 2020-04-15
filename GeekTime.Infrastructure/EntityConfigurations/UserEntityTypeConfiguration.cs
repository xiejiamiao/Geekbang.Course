using System;
using System.Collections.Generic;
using System.Text;
using GeekTime.Domain.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeekTime.Infrastructure.EntityConfigurations
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
