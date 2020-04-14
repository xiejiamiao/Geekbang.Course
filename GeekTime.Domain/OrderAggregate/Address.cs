using System;
using System.Collections.Generic;
using System.Text;
using GeekTime.Domain.Abstractions;

namespace GeekTime.Domain.OrderAggregate
{
    public class Address:ValueObject
    {
        public string Street { get; private set; }
        public string City { get; private set; }
        public string Zipcode { get; private set; }

        protected Address()
        {
            
        }

        public Address(string street,string city,string zipcode)
        {
            Street = street;
            City = city;
            Zipcode = zipcode;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Street;
            yield return City;
            yield return Zipcode;
        }
    }
}
