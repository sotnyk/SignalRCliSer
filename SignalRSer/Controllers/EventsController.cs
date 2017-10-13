using Common;
using Microsoft.AspNet.SignalR.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SignalRSer.Controllers
{
    [RoutePrefix("Events")]
    public class EventsController : ApiController
    {
        private readonly IConnectionManager _connectionManager;

        [Route("Fire/{toi}")]
        // GET api/values
        public void Fire([FromUri] TypeOfInterest toi)
        {

        }
    }
}
