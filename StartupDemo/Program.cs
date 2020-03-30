using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace StartupDemo
{
    /*
     * �γ�Ŀ�꣺����ASP.NET Core����������
     * ����ִ��˳��
     * ConfigureWebHostDefaults    ע����Ӧ�ó����Ҫ�ļ������������˵���õ�����������������
     * ConfigureHostConfiguration  ���ó�������ʱ��Ҫ�����ã�����˵��������ʱ����Ҫ�����Ķ˿ڡ���Ҫ������URL��ַ�ȣ���������̿���Ƕ�������Լ�����������ע�뵽���õĿ����ȥ
     * ConfigureAppConfiguration   Ƕ�������Լ��������ļ�����Ӧ�ó�������ȡ����Щ���ý������ں�����Ӧ�ó���ִ�й�����ÿ�������ȡ
     *
     * ConfigureServices/ConfigureLogging/Startup/Startup.ConfigureServices   ��Щ����������������ע�����ǵ�Ӧ�õ����
     * Startup.Configure  ע���м��������HttpContext�������������
     *
     * ������������Startup����඼���Ǳ���ģ�����ֱ��ConfigureWebHostDefaults������ֱ��ʹ��webBuilder����ConfigureServices��Configure��������
     * Startup�����ֻ���ô���ṹ���Ӻ���
     *
     * һ������Startup.ConfigureServices�ķ�����������ע�ᣬһ����Addxxx
     * ��Startup.Configure����ע����Щ�м�����������ȥ
     */



    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    Console.WriteLine("ConfigureWebHostDefaults");
                    //webBuilder.UseStartup<Startup>();
                    webBuilder.ConfigureServices(services =>
                    {
                        Console.WriteLine("Program.ConfigureServices");
                        services.AddControllers();
                    });
                    webBuilder.Configure(app =>
                    {
                        Console.WriteLine("Program.Configure");

                        app.UseHttpsRedirection();

                        app.UseRouting();

                        app.UseAuthorization();

                        app.UseStaticFiles();

                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapControllers();
                        });
                    });
                })
                .ConfigureServices(service =>
                {
                    Console.WriteLine("ConfigureServices");
                })
                .ConfigureAppConfiguration(builder =>
                {
                    Console.WriteLine("ConfigureAppConfiguration");
                })
                .ConfigureHostConfiguration(builder =>
                {
                    Console.WriteLine("ConfigureHostConfiguration");
                });
    }
}
