using System;
using System.Collections.Generic;
using System.Text;
using DimSum.Domain.Abstractions;

namespace DimSum.Domain.OrderAggregate
{
    public class Address : ValueObject
    {
        public string Street { get; }
        public string City { get; }
        public string Zipcode { get; }

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
