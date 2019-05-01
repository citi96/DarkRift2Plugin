using System.Collections.Generic;
using DR2Plugin.Interfaces.Messaging;

namespace DR2Plugin.Implementations.Messaging {
    public class Event : IMessage {
        public MessageType Type => MessageType.Request;
        public byte Code { get; }
        public int? SubCode { get; }
        public Dictionary<byte, object> Parameters { get; }

        public Event(byte code, Dictionary<byte, object> parameters, int? subCode) {
            Code = code;
            Parameters = parameters;
            SubCode = subCode;
        }
    }
}
