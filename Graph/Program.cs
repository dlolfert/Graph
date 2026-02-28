using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;

namespace Graph
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
        public static void WriteLog(string msg)
        {
            Console.WriteLine(msg);
            System.Diagnostics.Debug.WriteLine(msg);
        }
        public static void WriteLog(int msg)
        {
            Console.WriteLine(msg.ToString());
            System.Diagnostics.Debug.WriteLine(msg.ToString());
        }
        public static void WriteLog(decimal msg)
        {
            Console.WriteLine(msg.ToString());
            System.Diagnostics.Debug.WriteLine(msg.ToString());
        }
        public static void WriteLog(DateTime msg)
        {
            Console.WriteLine(msg.ToString("yyyy-MM-dd"));
            System.Diagnostics.Debug.WriteLine(msg.ToString("yyyy-MM-dd"));
        }
    }
}