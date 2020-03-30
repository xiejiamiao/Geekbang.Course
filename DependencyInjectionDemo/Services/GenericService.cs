using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DependencyInjectionDemo.Services
{
    public interface IGenericService<T>
    {
    }

    public class GenericService<T> : IGenericService<T>
    {
        private readonly T _data;

        public GenericService(T data)
        {
            _data = data;
        }
    }
}
