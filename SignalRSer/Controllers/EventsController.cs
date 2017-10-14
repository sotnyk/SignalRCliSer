using Common;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using SignalRSer.Hubs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace SignalRSer.Controllers
{
    [RoutePrefix("Events")]
    public class EventsController : ApiController
    {
        private readonly IConnectionManager _connectionManager;

        public EventsController(/*IConnectionManager connectionManager*/)
        {
            _connectionManager = GlobalHost.DependencyResolver.Resolve<IConnectionManager>();
            //_connectionManager = connectionManager;
            Trace.TraceInformation("EventsController()");
        }

        [Route("Fire/{toi}"), HttpGet]
        // GET api/values
        public async Task Fire([FromUri] TypeOfInterest toi)
        {
            var hub = _connectionManager.GetHubContext<UpdatesHub>();
            await hub.Clients.All.OnUpdated(toi);
        }
    }
}
