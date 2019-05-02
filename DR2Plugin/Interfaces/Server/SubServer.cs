using DarkRift.Server;
using DR2Plugin.Implementations.Messaging;
using DR2Plugin.Interfaces.Client;
using DR2Plugin.Interfaces.Messaging;
using GameCommon;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DR2Plugin.Interfaces.Server {
    public abstract class SubServer : ISubServer{
        protected IHandlerList<ISubServer> subServerHandlerList;

        public abstract byte SubCodeParameterCode { get; }
        public abstract IConnectionCollection<IClientPeer> ConnectionCollection { get; }
        public Plugin Plugin { get; }

        protected SubServer(Plugin plugin) {
            Plugin = plugin;
        }

        public abstract void ConnectPeer(IClientPeer peer);
        public abstract void DisconnectPeer(IClientPeer peer);

        protected void OnOperationRequest(object sender, MessageReceivedEventArgs e) {
            var peer = ConnectionCollection.GetPeers<IClientPeer>().FirstOrDefault(c => c.Client == e.Client);
            subServerHandlerList.Peer = peer;

            Console.WriteLine("Handling operation request");

            using(var message = e.GetMessage()) {
                using(var reader = message.GetReader()) {
                    var parameters =
                        MessageSerializerService.DeserializeObjectOfType<Dictionary<byte, object>>(reader.ReadString());
                    subServerHandlerList.HandleMessage(new Request((byte)e.Tag,
                        parameters.ContainsKey(SubCodeParameterCode)
                            ? (int?)Convert.ToInt32(parameters[SubCodeParameterCode])
                            : null, parameters), this);
                }
            }
        }
    }
}
