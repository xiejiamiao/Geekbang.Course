using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using DependencyInjectionAutofacDemo.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DependencyInjectionAutofacDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            //builder.RegisterType<MyService>().As<IMyService>(); //先注册具体的实现，然后再指明要标记成哪个服务的类型

            #region 命名注册

            //builder.RegisterType<MyServiceV2>().Named<IMyService>("service2");

            #endregion

            #region 属性注册

            //builder.RegisterType<MyNameService>(); //先注册属性类型
            //builder.RegisterType<MyServiceV2>().As<IMyService>().PropertiesAutowired(); //自动属性注册

            #endregion

            #region AOP
            //拦截器分两种类型，一种接口拦截器，一种是类拦截器，常用的是接口拦截器
            //builder.RegisterType<MyInterceptor>(); //1.注册拦截器
            //builder.RegisterType<MyServiceV2>().As<IMyService>().PropertiesAutowired().InterceptedBy(typeof(MyInterceptor)).EnableInterfaceInterceptors(); //注册MyServiceV2为IMyService类型，允许属性注册，同时指定拦截器为MyInterceptor，并且允许接口拦截器生效

            #endregion

            #region 子容器
            //Autofac具备给子容器进行命名的特性
            builder.RegisterType<MyNameService>().InstancePerMatchingLifetimeScope("myScope"); //把MyNameService注册到myScope这个子容器里面去，这意味着其他容器是获取不到这个对象的

            #endregion
        }

        public ILifetimeScope AutofacContainer { get; private set; }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            this.AutofacContainer = app.ApplicationServices.GetAutofacRoot();

            // 获取指定名字的实现类
            //var service = this.AutofacContainer.ResolveNamed<IMyService>("service2");
            //service.ShowCode();

            // 获取没有指定名字的实现类
            //var serviceNoName = this.AutofacContainer.Resolve<IMyService>();
            //serviceNoName.ShowCode();
            //var nameService = this.AutofacContainer.Resolve<MyNameService>();
            //if (nameService != null)
            //{

            //}

            //var serviceNamed = this.AutofacContainer.Resolve<IMyService>();
            //serviceNamed.ShowCode();

            #region 子容器

            using (var myScope = this.AutofacContainer.BeginLifetimeScope("myScope"))
            {
                var service0 = myScope.Resolve<MyNameService>();
                using (var scope = myScope.BeginLifetimeScope())
                {
                    var service1 = scope.Resolve<MyNameService>();
                    var service2 = scope.Resolve<MyNameService>();

                    Console.WriteLine($"service0=service1:{service0 == service1}");
                    Console.WriteLine($"service1=service2:{service1 == service2}");
                }
            }
            #endregion


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
