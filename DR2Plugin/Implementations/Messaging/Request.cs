using DR2Plugin.Interfaces.Messaging;
using System.Collections.Generic;

namespace DR2Plugin.Implementations.Messaging {
    public class Request : IMessage {
        public MessageType Type => MessageType.Request;
        public byte Code { get; }
        public int? SubCode { get; }
        public Dictionary<byte, object> Parameters { get; }

        public Request(byte code, int? subCode, Dictionary<byte, object> parameters) {
            Code = code;
            Parameters = parameters;
            SubCode = subCode;
        }
    }
}