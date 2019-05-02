using DR2Plugin.Data.Client;
using DR2Plugin.Implementations.Messaging;
using DR2Plugin.Interfaces.Client;
using DR2Plugin.Interfaces.Messaging;
using GameCommon;
using System;
using System.Collections.Generic;

namespace DR2Plugin.Handlers {
    public class TestRequestResponse : IHandler<IClientPeer> {
        public MessageType Type => MessageType.Request;
        public byte Code => (byte) MessageOperationCode.Test;
        public int? SubCode => (int) MessageSubCode.TestRequestResponse;

        public bool HandleMessage(IMessage message, IClientPeer subServer) {
            Console.WriteLine($"Handled message {Code} - {SubCode} for client {subServer.ClientData<UserData>().Id}");
            var response = new Response(Code, new Dictionary<byte, object>(), SubCode,
                "Response from Request for Response", 0);
            subServer.SendMessage(response);
            return true;
        }
    }
}