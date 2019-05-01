using DR2Plugin.Interfaces.Client;
using System.Collections.Generic;
using System.Linq;

namespace DR2Plugin.Implementations.Client {
    public class ClientConnectionCollection : IConnectionCollection<IClientPeer> {
        private readonly List<IClientPeer> client;

        public ClientConnectionCollection() {
            client = new List<IClientPeer>();
        }

        public void Clear() {
            client.Clear();
        }

        public void Connect(IClientPeer peer) {
            client.Add(peer);
        }

        public void Disconnect(IClientPeer peer) {
            client.Remove(peer);
        }

        public List<T> GetPeers<T>() where T : class, IClientPeer {
            return new List<T>(client.Cast<T>());
        }
    }
}
