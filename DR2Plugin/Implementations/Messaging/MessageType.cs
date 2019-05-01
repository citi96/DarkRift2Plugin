using System;

namespace DR2Plugin.Implementations.Messaging {
    [Flags]
    public enum MessageType {
        Request = 0x1,
        Response = 0x2,
        Async = 0x3
    }
}