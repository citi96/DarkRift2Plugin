using System.Collections.Generic;
using DR2Plugin.Interfaces.Messaging;

namespace DR2Plugin.Implementations.Messaging {
    public class Response : IMessage {
        public MessageType Type => MessageType.Response;
        public byte Code { get; }
        public int? SubCode { get; }
        public Dictionary<byte, object> Parameters { get; }

        //Photon specific at the moment
        public string DebugMessage { get; }
        public short ReturnCode { get; }

        public Response(byte code, Dictionary<byte, object> parameters, int? subCode) {
            Code = code;
            Parameters = parameters;
            SubCode = subCode;
        }

        public Response(byte code, Dictionary<byte, object> parameters, int? subCode, string debugMessage,
            short returnCode) : this(code, parameters, subCode) {
            DebugMessage = debugMessage;
            ReturnCode = returnCode;
        }
    }
}