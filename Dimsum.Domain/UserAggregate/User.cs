using System;
using System.Collections.Generic;
using System.Text;
using Dimsum.Domain.Abstractions;

namespace Dimsum.Domain.UserAggregate
{
    public class User : Entity<long>, IAggregateRoot
    {
        public string Username { get; }
        public string Password { get; }

        public User(string username,string password)
        {
            Username = username;
            Password = password;
        }
    }
}
