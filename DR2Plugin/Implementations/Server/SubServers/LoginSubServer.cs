using System;
using System.Collections.Generic;
using DarkRift.Server;
using DR2Plugin.Data.Client;
using DR2Plugin.Implementations.Client;
using DR2Plugin.Implementations.Messaging;
using DR2Plugin.Interfaces.Client;
using DR2Plugin.Interfaces.Messaging;
using DR2Plugin.Interfaces.Server;

namespace DR2Plugin.Implementations.Server.SubServers {
    public class LoginSubServer : ISubServer {
        private readonly IHandlerList<ISubServer> subServerHandlerList;

        public byte SubCodeParameterCode => 0;
        public IConnectionCollection<IClientPeer> ConnectionCollection { get; }
        public Plugin Plugin { get; }

        public LoginSubServer(IConnectionCollection<IClientPeer> connectionCollection, Plugin plugin,
            IHandlerList<ISubServer> subServerHandlerList) {
            this.subServerHandlerList = subServerHandlerList;
            ConnectionCollection = connectionCollection;
            Plugin = plugin;
        }

        public void ConnectPeer(IClientPeer peer) {
            Console.WriteLine($"Client Peer connected to {nameof(GetType)}");
            ConnectionCollection.Connect(peer);
            peer.Server = this;
        }

        public void DisconnectPeer(IClientPeer peer) {
            Console.WriteLine($"Client Peer {peer.ClientData<UserData>().Id} disconnected from {this}");
            ConnectionCollection.Disconnect(peer);
        }

        public void OnOperationRequest(MessageReceivedEventArgs e, Dictionary<byte, object> parameters, IClientPeer peer) {
            subServerHandlerList.Peer = peer;

            subServerHandlerList.HandleMessage(new Request((byte) e.Tag,
                parameters.ContainsKey(SubCodeParameterCode)
                    ? (int?) Convert.ToInt32(parameters[SubCodeParameterCode])
                    : null, parameters), this);
        }
    }
}