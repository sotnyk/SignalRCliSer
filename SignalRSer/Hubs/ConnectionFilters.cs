using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalRSer.Hubs
{
    public class ConnectionFilters
    {
        private readonly Dictionary<string, ClientDescriptor> _connections =
            new Dictionary<string, ClientDescriptor>();

        public int Count => _connections.Count;

        public void Add(string connectionId, Guid id, HashSet<TypeOfInterest> tois)
        {
            lock (_connections)
            {
                if (!_connections.ContainsKey(connectionId))
                {
                    _connections.Add(connectionId, new ClientDescriptor {
                        ConnectionId = connectionId,
                        Id = id,
                        Tois = tois,
                    });
                }
                else
                {
                    _connections[connectionId].Tois = tois;
                }
            }
        }

        internal IEnumerable<Guid> AllSubscribers()
        {
            lock (_connections)
            {
                return _connections.Select(kvp => kvp.Value.Id);
            }
        }

        internal IEnumerable<Guid> FindInterestedSubscribers(TypeOfInterest toi)
        {
            lock (_connections)
            {
                return _connections
                    .Where(kvp => kvp.Value.Tois.Contains(toi))
                    .Select(kvp => kvp.Value.Id).Distinct();
            }
        }

        public void Remove(String connectionId)
        {
            lock (_connections)
            {
                if (_connections.ContainsKey(connectionId))
                    _connections.Remove(connectionId);
            }
        }

        internal class ClientDescriptor
        {
            public string ConnectionId { get; set; }
            public Guid Id { get; set; }
            public HashSet<TypeOfInterest> Tois { get; set; }
        }
    }
}