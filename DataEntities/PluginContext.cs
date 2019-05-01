using DataEntities.Entities;
using System.Data.Entity;
//"Data Source=.;Initial Catalog=LoCDB;Integrated Security=true;"
//Server=151.40.205.68;Database=LoCDB;User Id=postgres;Password=MegaSwampertDigsHoles
namespace DataEntities {
    public class PluginContext : DbContext {
        public PluginContext() : base("Data Source=.;Initial Catalog=LoCDB;Integrated Security=true;") { }

       /* protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            //Configure default schema
            modelBuilder.HasDefaultSchema("public");
        }*/

        // Define Entities Here
        public DbSet<User> Users { get; set; }
        public DbSet<Deck> Decks { get; set; }
        public DbSet<Spell> Spells { get; set; }
        public DbSet<CardsOwnedByPlayer> CardsOwnedByPlayer { get; set; }
    }
}