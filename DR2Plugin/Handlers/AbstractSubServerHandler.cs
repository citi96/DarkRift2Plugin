using DR2Plugin.Implementations.Messaging;
using DR2Plugin.Interfaces.Client;
using DR2Plugin.Interfaces.Messaging;
using DR2Plugin.Interfaces.Server;

namespace DR2Plugin.Handlers {
    public abstract class AbstractSubServerHandler : IHandler<ISubServer> {
        protected IClientPeer peer;

        public abstract MessageType Type { get; }
        public abstract byte Code { get; }
        public abstract int? SubCode { get; }
        public abstract bool HandleMessage(IMessage message, ISubServer subServer);

        public bool HandleMessage(IMessage message, ISubServer subServer, IClientPeer peer) {
            this.peer = peer;
            return HandleMessage(message, subServer);
        }

    }
}
