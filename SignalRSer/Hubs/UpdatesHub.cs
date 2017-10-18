using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Common;
using System.Threading.Tasks;

namespace SignalRSer.Hubs
{
    public class UpdatesHub : Hub<IClient>
    {
        private readonly static ConnectionFilters _connectionFilters =
            new ConnectionFilters();

        public static IEnumerable<Guid> FindInterestedSubscribers(TypeOfInterest toi)
        {
            return _connectionFilters.FindInterestedSubscribers(toi);
        }

        public static IEnumerable<Guid> AllSubscribers()
        {
            return _connectionFilters.AllSubscribers();
        }

        public void OnUpdated(TypeOfInterest toi)
        {
            Clients.All.Updated(toi);
        }

        public void RegisterTOI(IDictionary<TypeOfInterest, IList<long>> tois)
        {

        }

        public override Task OnConnected()
        {
            if (Guid.TryParse(Context.QueryString["id"], out var id))
            {
                Groups.Add(Context.ConnectionId, id.ToString());
            }
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            if (Guid.TryParse(Context.QueryString["id"], out var id))
            {
                _connectionFilters.Remove(Context.ConnectionId);
            }
            return base.OnDisconnected(stopCalled);
        }

        public void SetTois(HashSet<TypeOfInterest> tois)
        {
            if (Guid.TryParse(Context.QueryString["id"], out var id))
            {
                _connectionFilters.Add(Context.ConnectionId, id, tois);
            }
        }
    }
}