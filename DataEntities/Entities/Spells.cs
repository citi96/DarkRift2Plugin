namespace DataEntities.Entities {
    public class Spell {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Rarity { get; set; }
        public long Class { get; set; }
        public long Expansion { get; set; }
    }
}
