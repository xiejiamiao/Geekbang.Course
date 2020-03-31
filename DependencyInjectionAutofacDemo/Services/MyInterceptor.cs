using System;
using Castle.DynamicProxy;

namespace DependencyInjectionAutofacDemo.Services
{
    public class MyInterceptor:IInterceptor
    {
        // IInterceptor 是Autofac面向切面最重要的一个接口，他可以把我们的逻辑注入到方法的切面里面去
        public void Intercept(IInvocation invocation)
        {
            Console.WriteLine($"Intercept before,Method:{invocation.Method.Name}");
            invocation.Proceed(); //这里是指具体方法的执行，如果这句不执行，就相当于把切面方法拦截了，让具体类的方法不执行
            Console.WriteLine($"Intercept after,Method:{invocation.Method.Name}");
        }
    }
}
