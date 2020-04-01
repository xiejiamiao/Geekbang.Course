using Microsoft.Extensions.Configuration;

namespace ConfigurationCustomDemo
{
    public static class MyConfigurationBuilderExtension
    {
        public static IConfigurationBuilder AddMyConfiguration(this IConfigurationBuilder builder)
        {
            builder.Add(new MyConfigurationSource());
            return builder;
        }
    }
}
