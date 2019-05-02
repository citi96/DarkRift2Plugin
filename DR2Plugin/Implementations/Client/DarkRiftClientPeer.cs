using DarkRift;
using DarkRift.Server;
using DR2Plugin.Data.Client;
using DR2Plugin.Implementations.Messaging;
using DR2Plugin.Interfaces.Client;
using DR2Plugin.Interfaces.Messaging;
using DR2Plugin.Interfaces.Server;
using GameCommon;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DR2Plugin.Implementations.Client {
    public class DarkRiftClientPeer : IClientPeer {
        public ISubServer Server { get; set; }
        public IClient Client { get; }

        private readonly IHandlerList<IClientPeer> handlerList;
        private readonly Dictionary<Type, IClientData> clientData;


        public DarkRiftClientPeer(ISubServer server, IHandlerList<IClientPeer> handlerList, IClient client,
            IEnumerable<IClientData> clientData) {
            Server = server;
            this.handlerList = handlerList;
            Client = client;
            this.clientData = new Dictionary<Type, IClientData>();

            foreach (var data in clientData) {
                this.clientData.Add(data.GetType(), data);
            }

            Server.Plugin.ClientManager.ClientDisconnected += OnDisconnect;
            Console.WriteLine("Created client peer.");
            Client.MessageReceived += OnOperationRequest;
        }

        public void Connect() {
            var userData = (UserData) clientData[typeof(UserData)];
            Console.WriteLine($"User {userData.Id} joined the game.");
        }

        public void SendMessage(IMessage message) {
            if (!message.Parameters.Keys.Contains(Server.SubCodeParameterCode)) {
                message.Parameters.Add(Server.SubCodeParameterCode, message.SubCode);
            }

            switch (message) {
                case Event _:
                    using (var writer = DarkRiftWriter.Create()) {
                        var parameters = new Dictionary<byte, object>() {
                            {(byte) MessageParameterCode.SubCodeParameterCode, message.SubCode}
                        };
                        var serializedParams = MessageSerializerService.SerializeObjectOfType(parameters);
                        writer.Write((string) serializedParams);
                        using (var msg = Message.Create(message.Code, writer)) {
                            Client.SendMessage(msg, SendMode.Reliable);
                        }
                    }

                    break;
                case Response response:
                    using (var writer = DarkRiftWriter.Create()) {
                        var serializedParams = MessageSerializerService.SerializeObjectOfType(response.Parameters);
                        writer.Write((string) serializedParams);
                        writer.Write(response.DebugMessage);
                        writer.Write(response.ReturnCode);
                        using (var msg = Message.Create(message.Code, writer)) {
                            Client.SendMessage(msg, SendMode.Reliable);
                        }
                    }

                    break;
            }
        }

        private void OnOperationRequest(object sender, MessageReceivedEventArgs e) {
            Console.WriteLine("Handling operation request");

            using (var message = e.GetMessage()) {
                using (var reader = message.GetReader()) {
                    var parameters =
                        MessageSerializerService.DeserializeObjectOfType<Dictionary<byte, object>>(reader.ReadString());
                    handlerList.HandleMessage(
                        new Request((byte) e.Tag,
                            parameters.ContainsKey(Server.SubCodeParameterCode)
                                ? (int?) Convert.ToInt32(parameters[Server.SubCodeParameterCode])
                                : null, parameters), this);
                }
            }
        }

        private void OnDisconnect(object sender, ClientDisconnectedEventArgs e) {
            Server.DisconnectPeer(this);
            var userData = (UserData) clientData[typeof(UserData)];
            Console.WriteLine($"User {userData.Id} left the game.");
        }

        T IClientPeer.ClientData<T>() {
            clientData.TryGetValue(typeof(T), out var result);
            return result as T;
        }
    }
}