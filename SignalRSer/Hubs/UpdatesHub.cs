using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Common;

namespace SignalRSer.Hubs
{
    public class UpdatesHub : Hub<IClient>
    {
        public void OnUpdated(TypeOfInterest toi)
        {
            Clients.All.Updated(toi);
        }

        public void RegisterTOI(IDictionary<TypeOfInterest, IList<long>> tois)
        {

        }
    }
}