using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace OptionsDemo.Services
{
    public interface IOrderService
    {
        int ShowMaxOrderCount();
    }

    public class OrderService:IOrderService
    {
        /*
        private readonly IOptions<OrderServiceOptions> _options;

        public OrderService(IOptions<OrderServiceOptions> options)
        {
            _options = options;
        }
        */

        /*
         private readonly IOptionsSnapshot<OrderServiceOptions> _options;
        public OrderService(IOptionsSnapshot<OrderServiceOptions> options)
        {
            _options = options;
        }
        */

        
        private readonly IOptionsMonitor<OrderServiceOptions> _options;
        public OrderService(IOptionsMonitor<OrderServiceOptions> options)
        {
            _options = options;
            this._options.OnChange(changedOptions =>
            {
                Console.WriteLine($"配置发生了变化,新值为:{changedOptions.MaxOrderCount}");
            });
        }


        public int ShowMaxOrderCount()
        {
            //return _options.Value.MaxOrderCount;
            return _options.CurrentValue.MaxOrderCount;
        }
        
    }

    public class OrderServiceOptions
    {
        [Range(1,20)]
        public int MaxOrderCount { get; set; } = 100;
    }

    public class OrderServiceValidateOptions : IValidateOptions<OrderServiceOptions>
    {
        public ValidateOptionsResult Validate(string name, OrderServiceOptions options)
        {
            if (options.MaxOrderCount > 100)
            {
                return ValidateOptionsResult.Fail("MaxOrderCount 不能大于100");
            }
            else
            {
                return ValidateOptionsResult.Success;
            }
        }
    }
}
