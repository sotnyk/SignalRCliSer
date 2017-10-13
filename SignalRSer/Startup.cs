﻿using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;
using System.Security;

[assembly: OwinStartup(typeof(SignalRSer.Startup))]
namespace SignalRSer
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Any connection or hub wire up and configuration should go here
            var hubConfiguration = new HubConfiguration();
            hubConfiguration.EnableDetailedErrors = true;
            //hubConfiguration.EnableJavaScriptProxies = false;
            app.MapSignalR(hubConfiguration);
        }
    }
}