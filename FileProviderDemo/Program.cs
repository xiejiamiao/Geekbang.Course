using System;
using Microsoft.Extensions.FileProviders;

namespace FileProviderDemo
{
    class Program
    {
        static void Main(string[] args)
        {

            var phyProvider = new PhysicalFileProvider(AppDomain.CurrentDomain.BaseDirectory);
            var contents = phyProvider.GetDirectoryContents("/");
            foreach (var item in contents)
            {
                Console.WriteLine(item.Name);
            }

            var embProvider = new EmbeddedFileProvider(typeof(Program).Assembly);
            var html = embProvider.GetFileInfo("emb.html");
            Console.WriteLine($"获取到嵌入式文件:{html.Exists}");

            Console.WriteLine("=====↓CompositeFileProvider↓=====");

            var compositeProvider = new CompositeFileProvider(phyProvider, embProvider);
            var comContent = compositeProvider.GetDirectoryContents("/");
            foreach (var item in comContent)
            {
                Console.WriteLine(item.Name);
            }

            Console.WriteLine("=====END=====");
        }
    }
}
