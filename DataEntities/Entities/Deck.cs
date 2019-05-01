namespace DataEntities.Entities {
    public class Deck {
        public long Id { get; set; }
        public long Owner { get; set; }
        public string Name { get; set; }
        public long Class { get; set; }

        public User User { get; set; }
    }
}
