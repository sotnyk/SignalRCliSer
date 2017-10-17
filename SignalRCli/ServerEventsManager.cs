using Common;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SignalRCli
{
    class ServerEventsManager
    {
        HubConnection _hubConnection;
        IHubProxy _hubProxy;
        private Task _connectionTask;
        private CancellationTokenSource _connectionTaskCT;
        Guid _id = Guid.NewGuid();
        private object _sync = new object();

        public ServerEventsManager(string apiUri)
        {
            IDictionary<string, string> queryString = new Dictionary<string, string>();
            queryString.Add("id", _id.ToString());
            _hubConnection = new HubConnection(apiUri, queryString);
            _hubConnection.StateChanged += HubConnection_StateChanged;
            _hubConnection.Closed += HubConnection_Closed;
            _hubConnection.ConnectionSlow += HubConnection_ConnectionSlow;
            _hubConnection.Error += HubConnection_Error;
            _hubConnection.Received += HubConnection_Received;
            _hubConnection.Reconnected += HubConnection_Reconnected;
            _hubConnection.Reconnecting += HubConnection_Reconnecting;
            _hubProxy = _hubConnection.CreateHubProxy("UpdatesHub");
            _hubProxy.On<TypeOfInterest>(nameof(IClient.Updated), OnUpdated);
        }

        private static void OnUpdated(TypeOfInterest toi)
        {
            Console.WriteLine($"Changed objects: " + toi.ToString());
        }

        public void Start()
        {
            lock (_sync)
            {
                if (_connectionTaskCT != null)
                    throw new ThreadStateException("Can't start already started connection.");
                _connectionTaskCT = new CancellationTokenSource();
                _connectionTask = new Task(ConnectionTaskBody, _connectionTaskCT.Token);
                _connectionTask.Start();
            }
        }

        private void ConnectionTaskBody()
        {
            var token = _connectionTaskCT.Token;
            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (_hubConnection != null && _hubConnection.State == ConnectionState.Disconnected)
                    {
                        _hubConnection.Start().Wait();
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());
                }
                try
                {
                    Task.Delay(2000, token).Wait();
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());
                }
            }
        }

        public void Stop()
        {
            lock (_sync)
            {
                if (_connectionTaskCT == null)
                    throw new ThreadStateException("Can't stop already stopped connection.");
                _connectionTaskCT.Cancel();
                _connectionTask.Wait();
                try
                {
                    _hubConnection.Stop();
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());
                }
                Trace.TraceInformation("Connection stopped.");
            }
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

        private void HubConnection_StateChanged(StateChange stateChange)
        {
            Console.WriteLine($"State changed from '{stateChange.OldState}' to '{stateChange.NewState}'.");
            if (stateChange.NewState == ConnectionState.Connected)
            {
                Console.WriteLine($"Updates subscription to {nameof(TypeOfInterest.Patients)} and {nameof(TypeOfInterest.ChatMessages)}.");
                _hubProxy.Invoke("SetTois", new HashSet<TypeOfInterest> { TypeOfInterest.Patients, TypeOfInterest.ChatMessages });
                Console.WriteLine($"Subscription has been updated.");
            }
        }
    }
}
