﻿using DarkRift.Server;
using DR2Plugin.Data.Client;
using DR2Plugin.Handlers;
using DR2Plugin.Implementations.Client;
using DR2Plugin.Implementations.Messaging;
using DR2Plugin.Interfaces.Client;
using DR2Plugin.Interfaces.Server;
using DR2Plugin.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DR2Plugin.Implementations.Server.SubServers {
    public class LoginSubServer : SubServer {
        public override byte SubCodeParameterCode => 0;
        public override IConnectionCollection<IClientPeer> ConnectionCollection { get; }

        public LoginSubServer(IAuthorizationService authService, Plugin plugin) : base(plugin) {
            ConnectionCollection = new ClientConnectionCollection();
            var loginHandlers = new List<AbstractSubServerHandler>
                {new LoginAccountCreationHandler(authService), new LoginAuthorizationHandler(authService)};
            subServerHandlerList = new SubServerHandlerList(loginHandlers);
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