using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DependencyInjectionDemo.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DependencyInjectionDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region ע�᲻ͬ�������ڵķ��񣨷���ע�����

            // ע��Singleton����
            services.AddSingleton<IMySingletonService, MySingletonService>();
            // ע��Scope����
            services.AddScoped<IMyScopeService, MyScopeService>();
            // ע��Transient����
            services.AddTransient<IMyTransientService, MyTransientService>();

            #endregion


            #region ��ʽע�����(��SingltonģʽΪ��)

            services.AddSingleton<IOrderService>(new OrderService()); //ֱ��ע��ʵ��
            services.AddSingleton<IOrderService>(serviceProvider =>
            {
                // �����ǿ���ʹ��serviceProvider�����εģ�Ҳ����ζ�ſ��Դ��������ȡ�������Ȼ�������װ���õ�����������Ҫ��ʵ��ʵ�������԰ѹ�������ƵıȽϸ���
                // ����˵���ǵ�ʵ�����������������������һ��������
                // ������������������һ��������װ����ԭ�е�ʵ�ֵ�ʱ��
                return new OrderServiceEx();
            }); //�ù���ģʽע��ʵ��

            #endregion

            #region ����ע��

            // ����ע����ָ��������Ѿ�ע����ˣ��Ͳ���ע��
            //services.TryAddSingleton<IOrderService, OrderServiceEx>(); //�������ӿ��Ѿ���ע����ˣ����ٽ���ע��
            //services.TryAddEnumerable(ServiceDescriptor.Singleton<IOrderService,OrderService>());  //�������ӿ�+���ʵ����ע���ˣ�����ע�᣻�������ӿ��Ѿ�ע��������ʵ���࣬�����ʵ��һ���ӿ��ж��ʵ����Ĳ���

            #endregion

            #region �Ƴ����滻ע�����

            //services.RemoveAll<IOrderService>();
            services.Replace(ServiceDescriptor.Singleton<IOrderService, OrderServiceEx>()); //�滻ָ���ӿڵ�ʵ���࣬ͬʱҲ���滻�÷������������

            #endregion


            #region ���ͷ���ע��

            services.AddSingleton(typeof(IGenericService<>), typeof(GenericService<>));

            #endregion


            services.AddControllers();
        }

// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
