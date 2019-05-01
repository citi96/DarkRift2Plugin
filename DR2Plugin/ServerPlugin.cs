using DarkRift.Server;
using DR2Plugin.Data.Client;
using DR2Plugin.Implementations.Client;
using DR2Plugin.Implementations.Messaging;
using DR2Plugin.Implementations.Services.Authorization;
using DR2Plugin.Interfaces.Client;
using DR2Plugin.Interfaces.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DR2Plugin.Handlers;
using DR2Plugin.Implementations.Server;
using DR2Plugin.Implementations.Server.SubServers;
using DR2Plugin.Interfaces.Server;

namespace DR2Plugin {
    public class ServerPlugin : Plugin {
        public override Version Version => new Version(0, 0, 1);
        public override bool ThreadSafe => false;


        private readonly IHandlerList<IClientPeer> clientHandlerList;
        private readonly IHandlerList<ISubServer> subServerHandlerList;
        private readonly IConnectionCollection<ISubServer> subServerCollection = new SubServerConnectionCollection();

        public ServerPlugin(PluginLoadData pluginLoadData) : base(pluginLoadData) {
            var authService = new UserSaltedPassAuthorizationService();

            var handlers = from t in Assembly.GetAssembly(GetType()).GetTypes()
                    .Where(t => t.GetInterfaces().Contains(typeof(IHandler<IClientPeer>)))
                select Activator
                    .CreateInstance(t) as IHandler<IClientPeer>;

            var loginHandlers = new List<AbstractSubServerHandler>
                {new LoginAccountCreationHandler(authService), new LoginAuthorizationHandler(authService)};

            subServerHandlerList = new SubServerHandlerList(loginHandlers);
            clientHandlerList = new ClientHandlerList(handlers);

            CreateSubServers();
            ClientManager.ClientConnected += OnConnect;
        }

        private void CreateSubServers() {
            subServerCollection.Connect(
                new LoginSubServer(new ClientConnectionCollection(), this, subServerHandlerList));
        }

        private void OnConnect(object sender, ClientConnectedEventArgs e) {
            var loginSubServer = subServerCollection.GetPeers<LoginSubServer>().FirstOrDefault();

            loginSubServer?.ConnectPeer(new DarkRiftClientPeer(loginSubServer, clientHandlerList, e.Client,
                new List<IClientData> {new UserData()}));
        }
    }
}