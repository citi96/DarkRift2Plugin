using DR2Plugin.Interfaces.Client;

namespace DR2Plugin.Interfaces.Messaging {
    public interface IHandlerList<in T> {
        IClientPeer Peer { set; }

        bool HandleMessage(IMessage message, T clientPeer);
    }
}
