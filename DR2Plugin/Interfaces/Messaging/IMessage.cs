using System.Collections.Generic;
using DR2Plugin.Implementations.Messaging;

namespace DR2Plugin.Interfaces.Messaging {
    public interface IMessage {
        MessageType Type { get; }
        byte Code { get; }
        int? SubCode { get; }
        Dictionary<byte, object> Parameters { get; }
    }
}
