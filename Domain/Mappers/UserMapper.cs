using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DataEntities;
using Domain.Domain;
using User = Domain.Domain.User;

namespace Domain.Mappers {
    public class UserMapper : MapperBase<User> {
        protected override User Delete(User domainObject) {
            if (domainObject == null) {
                throw new ArgumentNullException(nameof(domainObject));
            }

            DeleteNow(domainObject.Id);
            return domainObject;
        }

        protected override void DeleteNow(long id) {
            using (var entities = new PluginContext()) {
                var entity = new DataEntities.Entities.User {
                    Id = id
                };
                //Gets the character list and make sure this object exists in the list of objects.
                entities.Users.Attach((entity));
                // Remove the character from the database
                entities.Users.Remove(entity);
                entities.SaveChanges();
            }
        }

        protected override User Insert(User domainObject) {
            using (var entities = new PluginContext()) {
                var entity = new DataEntities.Entities.User();
                Map(domainObject, entity);
                entities.Users.Add((entity));
                domainObject = SaveChanges(entities, entity);
            }

            return domainObject;
        }

        protected override User Update(User domainObject) {
            if (domainObject == null) {
                throw new ArgumentNullException(nameof(domainObject));
            }

            // Pull out id because we'll be using it in a lambda that might be deferred when calling
            // and the thread may not have access to the domain object's context.
            long id = domainObject.Id;
            using (var entities = new PluginContext()) {
                var entity = entities.Users.Include(e => e.Decks).Include(e => e.PlayerCards)
                    .FirstOrDefault(e => e.Id == id);
                if (entity != null) {
                    Map(domainObject, entity);
                    domainObject = SaveChanges(entities, entity);
                }
            }

            return domainObject;
        }

        private User SaveChanges(PluginContext entities, DataEntities.Entities.User entity) {
            // Save everything in the context (unit of work means it should only be this entity and anything it contains).
            entities.SaveChanges();
            // Reload what the database database on the ID that was modified.
            return Fetch(entity.Id);
        }

        protected override IList<User> Fetch() {
            using (var entities = new PluginContext()) {
                // AsNoTracking do not cache the entities in EF, order the entities by ID, execute the query and return a list.
                // Using the list of entities, create new DomainBase Characters
                return entities.Users.AsNoTracking().OrderBy(e => e.Id).ToList()
                    .Select(e => new User(e.Id, e.Username, e.Hash, e.Salt, e.Email, e.Souls, e.Skulls)).ToList();
            }
        }

        protected override User Fetch(long id) {
            User userObject = null;
            using (var entities = new PluginContext()) {
                var entity = entities.Users
                    //.Include(e => e.Stats) e.g.
                    .FirstOrDefault(e => e.Id == id);

                if (entity != null) {
                    // Load data and extra data, such as linked objects or XML data, etc...
                    userObject = new User(entity.Id, entity.Username, entity.Hash, entity.Salt, entity.Email, entity.Souls, entity.Skulls);
                }
            }

            return userObject;
        }

        /// <inheritdoc />
        /// <summary>
        /// One way mapping of all data in the domain object to the entity for adding/updating.
        /// </summary>
        /// <param name="domainObject"></param>
        /// <param name="entity"></param>
        protected override void Map(User domainObject, object entity) {
            try {
                if (entity is DataEntities.Entities.User userEntity) {
                    // Map all fields from thedomain object to the entity except the ID if it isn't allowed to change (most IDs should NEVER change).
                    //characterEntity.Id = domainObject.Id;
                    userEntity.Username = domainObject.Username;
                    userEntity.Hash = domainObject.Hash;
                    userEntity.Salt = domainObject.Salt;
                    userEntity.Email = domainObject.Email;
                }
            }
            catch (ArgumentNullException e) {
                Console.WriteLine(e);
                throw new ArgumentNullException(nameof(domainObject));
            }
            catch (ArgumentOutOfRangeException e) {
                Console.WriteLine(e);
                throw new ArgumentOutOfRangeException(nameof(domainObject));
            }
        }

        public static IList<Deck> LoadDecks(User domainObject) {
            if (domainObject == null) {
                throw new ArgumentNullException(nameof(domainObject));
            }

            long id = domainObject.Id;
            var decks = new List<Deck>();

            using (var entities = new PluginContext()) {
                var query = entities.Decks.Where(e => e.Owner == id);

                foreach (var deck in query) {
                    decks.Add(new Deck(deck.Id, deck.Owner, deck.Name, deck.Class));
                }
            }

            return decks;
        }

        public static IList<CardsOwnedByPlayer> LoadCardsOwnedByPlayer(User domainObject) {
            if (domainObject == null) {
                throw new ArgumentNullException(nameof(domainObject));
            }

            long id = domainObject.Id;
            var playerCards = new List<CardsOwnedByPlayer>();

            using (var entities = new PluginContext()) {
                var query = entities.CardsOwnedByPlayer.Where(e => e.Owner == id);

                foreach (var card in query) {
                    playerCards.Add(new CardsOwnedByPlayer(card.Owner, card.CardId, card.Amount));
                }
            }

            return playerCards;
        }

        public static User LoadByUsername(string username) {
            User userObject = null;
            using (var entities = new PluginContext()) {
                var entity = entities.Users
                    //.Include(e => e.Stats) e.g.
                    .FirstOrDefault(e => e.Username == username);

                if (entity != null) {
                    // Load data and extra data, such as linked objects or XML data, etc...
                    userObject = new User(entity.Id, entity.Username, entity.Hash, entity.Salt, entity.Email, entity.Souls, entity.Skulls);
                }
            }

            return userObject;
        }
    }
}