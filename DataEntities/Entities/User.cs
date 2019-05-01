using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataEntities.Entities {
    public class User {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Hash { get; set; }
        public string Salt { get; set; }
        public string Email { get; set; }
        public int Souls { get; set; }
        public int Skulls { get; set; }

        public ICollection<CardsOwnedByPlayer> PlayerCards { get; set; }
        public ICollection<Deck> Decks { get; set; }
    }
}
