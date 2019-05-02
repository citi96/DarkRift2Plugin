using DarkRift.Server;
using DR2Plugin.Interfaces.Messaging;
using DR2Plugin.Interfaces.Server;

namespace DR2Plugin.Interfaces.Client {
    public interface IClientPeer {
        ISubServer Server { get; set; }
        IClient Client { get; }

        void Connect();
        T ClientData<T>() where T : class, IClientData;
        void SendMessage(IMessage message);
    }
}
