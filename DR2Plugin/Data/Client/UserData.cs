using DR2Plugin.Interfaces.Client;

namespace DR2Plugin.Data.Client {
    public class UserData : IClientData {
        public long Id { get; set; }
        public long Heads { get; set; }
        public long Skulls { get; set; }
    }
}
