using System;
using System.Collections.Generic;
using System.Text;
using Dimsum.Domain.Abstractions;

namespace Dimsum.Domain.OrderAggregate
{
    public class Address:ValueObject
    {
        public string Province { get; }
        public string City { get; }
        public string District { get; }
        public string Detail { get; }
        public string Zipcode { get; }

        public Address(string province, string city, string district, string detail, string zipcode)
        {
            Province = province;
            City = city;
            District = district;
            Detail = detail;
            Zipcode = zipcode;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Province;
            yield return City;
            yield return District;
            yield return Detail;
            yield return Zipcode;
        }
    }
}
