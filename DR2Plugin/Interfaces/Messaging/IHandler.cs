using DR2Plugin.Implementations.Messaging;

namespace DR2Plugin.Interfaces.Messaging {
    public interface IHandler<in T> {
        MessageType Type { get; }
        byte Code { get; }
        int? SubCode { get; }

        bool HandleMessage(IMessage message, T subServer);
    }
}