using System.Collections.Generic;

namespace DR2Plugin.Interfaces.Client {
    public interface IConnectionCollection<in TPeer> {
        void Connect(TPeer peer);
        void Disconnect(TPeer peer);
        void Clear();
        List<T> GetPeers<T>() where T : class, TPeer;
    }
}