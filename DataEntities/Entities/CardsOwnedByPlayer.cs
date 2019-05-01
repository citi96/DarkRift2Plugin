using System.ComponentModel.DataAnnotations;

namespace DataEntities.Entities {
    public class CardsOwnedByPlayer {
        [Key]
        public long Owner { get; set; }
        public long CardId { get; set; }
        public int Amount { get; set; }

        public User User { get; set; }
    }
}
