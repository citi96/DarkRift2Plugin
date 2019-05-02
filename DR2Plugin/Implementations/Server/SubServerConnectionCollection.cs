using System.Collections.Generic;
using System.Linq;
using DR2Plugin.Interfaces.Client;
using DR2Plugin.Interfaces.Server;

namespace DR2Plugin.Implementations.Server {
    public class SubServerConnectionCollection : IConnectionCollection<ISubServer> {
        private readonly List<ISubServer> subServers;

        public SubServerConnectionCollection() {
            subServers = new List<ISubServer>();
        }

        public void Connect(ISubServer peer) {
            subServers.Add(peer);
        }

        public void Disconnect(ISubServer peer) {
            subServers.Remove(peer);
        }

        public void Clear() {
            subServers.Clear();
        }

        public List<T> GetPeers<T>() where T : class, ISubServer {
            var list = subServers.Where(s => s.GetType() == typeof(T)).ToList().Cast<T>();
            return new List<T>(list);
        }
    }
}
