using System.Collections.Generic;
using DarkRift.Server;
using DR2Plugin.Interfaces.Client;
using DR2Plugin.Interfaces.Messaging;

namespace DR2Plugin.Interfaces.Server {
    public interface ISubServer {
        byte SubCodeParameterCode { get; }
        IConnectionCollection<IClientPeer> ConnectionCollection { get; }
        Plugin Plugin { get; }

        void ConnectPeer(IClientPeer peer);
        void DisconnectPeer(IClientPeer peer);
    }
}
