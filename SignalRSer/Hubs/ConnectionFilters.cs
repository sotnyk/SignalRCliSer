using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalRSer.Hubs
{
    public class ConnectionFilters
    {
        private readonly Dictionary<Guid, HashSet<TypeOfInterest>> _connections =
            new Dictionary<Guid, HashSet<TypeOfInterest>>();

        public int Count => _connections.Count;

        public void Add(Guid id, HashSet<TypeOfInterest> tois)
        {
            lock (_connections)
            {
                if (!_connections.ContainsKey(id))
                {
                    _connections.Add(id, tois);
                }
                else
                {
                    _connections[id] = tois;
                }
            }
        }

        internal IEnumerable<Guid> AllSubscribers()
        {
            lock (_connections)
            {
                return _connections.Select(kvp => kvp.Key);
            }
        }

        internal IEnumerable<Guid> FindInterestedSubscribers(TypeOfInterest toi)
        {
            lock (_connections)
            {
                return _connections.Where(kvp => kvp.Value.Contains(toi)).Select(kvp => kvp.Key);
            }
        }

        public IEnumerable<Guid> GetConnectionIds()
        {
            return _connections.Keys;
        }

        public void Remove(Guid id)
        {
            lock (_connections)
            {
                if (_connections.ContainsKey(id))
                    _connections.Remove(id);
            }
        }

        //public void 
    }
}