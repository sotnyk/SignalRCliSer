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

            var hubConnection = new HubConnection(settings.ApiUri);
            hubConnection.StateChanged += HubConnection_StateChanged;
            hubConnection.Closed += HubConnection_Closed;
            hubConnection.ConnectionSlow += HubConnection_ConnectionSlow;
            hubConnection.Error += HubConnection_Error;
            hubConnection.Received += HubConnection_Received;
            hubConnection.Reconnected += HubConnection_Reconnected;
            hubConnection.Reconnecting += HubConnection_Reconnecting;
            IHubProxy hubProxy = hubConnection.CreateHubProxy("UpdatesHub");
            hubProxy.On<TypeOfInterest>(nameof(IClient.Updated), toi => Console.WriteLine("Changed ", toi.ToString()));
            
            hubConnection.Start().Wait();

            Console.ReadLine();
        }

        private static void HubConnection_Reconnecting()
        {
            Console.WriteLine("Start reconnecting.");
        }

        private static void HubConnection_Reconnected()
        {
            Console.WriteLine("Reconnected after timeout.");
        }

        private static void HubConnection_Received(string data)
        {
            Console.WriteLine("Received data: " + data);
        }

        private static void HubConnection_Error(Exception exception)
        {
            Console.WriteLine("Error: " + exception);
        }

        private static void HubConnection_ConnectionSlow()
        {
            Console.WriteLine("Connection is slow.");
        }

        private static void HubConnection_Closed()
        {
            Console.WriteLine("Connection closed.");
        }

        private static void HubConnection_StateChanged(StateChange stateChange)
        {
            Console.WriteLine($"State changed from '{stateChange.OldState}' to '{stateChange.NewState}'.");
        }
    }
}
