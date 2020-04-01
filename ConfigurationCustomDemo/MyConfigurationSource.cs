using Microsoft.Extensions.Configuration;

namespace ConfigurationCustomDemo
{
    internal class MyConfigurationSource:IConfigurationSource
    {
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new MyConfigurationProvider();
        }
    }
}
