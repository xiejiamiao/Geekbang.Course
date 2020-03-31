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
            //builder.RegisterType<MyService>().As<IMyService>(); //��ע������ʵ�֣�Ȼ����ָ��Ҫ��ǳ��ĸ����������

            #region ����ע��

            //builder.RegisterType<MyServiceV2>().Named<IMyService>("service2");

            #endregion

            #region ����ע��

            //builder.RegisterType<MyNameService>(); //��ע����������
            //builder.RegisterType<MyServiceV2>().As<IMyService>().PropertiesAutowired(); //�Զ�����ע��

            #endregion

            #region AOP
            //���������������ͣ�һ�ֽӿ���������һ�����������������õ��ǽӿ�������
            //builder.RegisterType<MyInterceptor>(); //1.ע��������
            //builder.RegisterType<MyServiceV2>().As<IMyService>().PropertiesAutowired().InterceptedBy(typeof(MyInterceptor)).EnableInterfaceInterceptors(); //ע��MyServiceV2ΪIMyService���ͣ���������ע�ᣬͬʱָ��������ΪMyInterceptor����������ӿ���������Ч

            #endregion

            #region ������
            //Autofac�߱�����������������������
            builder.RegisterType<MyNameService>().InstancePerMatchingLifetimeScope("myScope"); //��MyNameServiceע�ᵽmyScope�������������ȥ������ζ�����������ǻ�ȡ������������

            #endregion
        }

        public ILifetimeScope AutofacContainer { get; private set; }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            this.AutofacContainer = app.ApplicationServices.GetAutofacRoot();

            // ��ȡָ�����ֵ�ʵ����
            //var service = this.AutofacContainer.ResolveNamed<IMyService>("service2");
            //service.ShowCode();

            // ��ȡû��ָ�����ֵ�ʵ����
            //var serviceNoName = this.AutofacContainer.Resolve<IMyService>();
            //serviceNoName.ShowCode();
            //var nameService = this.AutofacContainer.Resolve<MyNameService>();
            //if (nameService != null)
            //{

            //}

            //var serviceNamed = this.AutofacContainer.Resolve<IMyService>();
            //serviceNamed.ShowCode();

            #region ������

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
