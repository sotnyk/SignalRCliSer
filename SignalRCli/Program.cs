using Common;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace SignalRCli
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World, SignalR!");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            var config = builder.Build();
            var settings = new Settings();
            config.GetSection("Api").Bind(settings);

            Console.WriteLine("Api URI: " + settings.ApiUri);

            var seManager = new ServerEventsManager(settings.ApiUri);           
            seManager.Start();

            Console.WriteLine("Connection manager started. Press <Enter> to stop.");
            Console.WriteLine($"Use {settings.ApiUri}Events/Fire/0..3 to check events broadcast.");
            Console.WriteLine($"    {settings.ApiUri}Events/AllSubscribers to see all active subscribers.");
            Console.WriteLine();

            Console.ReadLine();

            seManager.Stop();
            Console.WriteLine("Connection manager stopped. Application terminated.");
        }
    }
}
