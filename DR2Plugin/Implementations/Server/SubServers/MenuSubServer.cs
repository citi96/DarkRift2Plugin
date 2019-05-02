using DarkRift.Server;
using DR2Plugin.Data.Client;
using DR2Plugin.Handlers;
using DR2Plugin.Implementations.Client;
using DR2Plugin.Implementations.Messaging;
using DR2Plugin.Interfaces.Client;
using DR2Plugin.Interfaces.Server;
using System;
using System.Collections.Generic;

namespace DR2Plugin.Implementations.Server.SubServers {
    public class MenuSubServer : SubServer {
        public override byte SubCodeParameterCode => 0;
        public override IConnectionCollection<IClientPeer> ConnectionCollection { get; }

        public MenuSubServer(Plugin plugin) : base(plugin) {
            ConnectionCollection = new ClientConnectionCollection();
            var menuHandlers = new List<AbstractSubServerHandler>{};
            subServerHandlerList = new SubServerHandlerList(menuHandlers);
        }

        public override void ConnectPeer(IClientPeer peer) {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Client Peer connected to {GetType().Name}");
            Console.ResetColor();
            ConnectionCollection.Connect(peer);
            peer.Server = this;
            peer.Client.MessageReceived += OnOperationRequest;
        }

        public override void DisconnectPeer(IClientPeer peer) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Client Peer {peer.ClientData<UserData>().Id} disconnected from {GetType().Name}");
            Console.ResetColor();
            ConnectionCollection.Disconnect(peer);
            peer.Client.MessageReceived -= OnOperationRequest;
        }
    }
}