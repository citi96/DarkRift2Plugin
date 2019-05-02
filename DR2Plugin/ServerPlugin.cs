using DarkRift.Server;
using DR2Plugin.Data.Client;
using DR2Plugin.Implementations.Client;
using DR2Plugin.Implementations.Messaging;
using DR2Plugin.Implementations.Server;
using DR2Plugin.Implementations.Server.SubServers;
using DR2Plugin.Implementations.Services.Authorization;
using DR2Plugin.Interfaces.Client;
using DR2Plugin.Interfaces.Messaging;
using DR2Plugin.Interfaces.Server;
using DR2Plugin.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DR2Plugin {
    public class ServerPlugin : Plugin {
        public override Version Version => new Version(0, 0, 1);
        public override bool ThreadSafe => false;

        private readonly IHandlerList<IClientPeer> clientHandlerList;
        private readonly IConnectionCollection<ISubServer> subServerCollection = new SubServerConnectionCollection();
        private readonly IAuthorizationService authService = new UserSaltedPassAuthorizationService();

        public ServerPlugin(PluginLoadData pluginLoadData) : base(pluginLoadData) {
            

            var handlers = from t in Assembly.GetAssembly(GetType()).GetTypes()
                    .Where(t => t.GetInterfaces().Contains(typeof(IHandler<IClientPeer>)))
                select Activator
                    .CreateInstance(t) as IHandler<IClientPeer>;
            clientHandlerList = new ClientHandlerList(handlers);

            CreateSubServers();
            ClientManager.ClientConnected += OnConnect;
        }

        public List<T> GetSubServerOfType<T>() where T : class, ISubServer {
            return subServerCollection.GetPeers<T>().Where(s => s.GetType() == typeof(T)).ToList();
        }

        private void CreateSubServers() {
            subServerCollection.Connect(new LoginSubServer(authService, this));
            subServerCollection.Connect(new MenuSubServer(this));
        }

        private void OnConnect(object sender, ClientConnectedEventArgs e) {
            var loginSubServer = subServerCollection.GetPeers<LoginSubServer>().FirstOrDefault();

            loginSubServer?.ConnectPeer(new DarkRiftClientPeer(loginSubServer, clientHandlerList, e.Client,
                new List<IClientData> {new UserData()}));
        }
    }
}