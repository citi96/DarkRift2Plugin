using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Domain.Mappers;

namespace Domain.Domain {
    [Serializable]
    public class User : DomainBase {
        #region Private Field

        private long id;
        private string username;
        private string hash;
        private string salt;
        private string email;
        private int skulls;
        private int souls;

        private List<CardsOwnedByPlayer> playerCards;
        private List<Deck> decks;

        #endregion

        #region Properties

        public long Id {
            get => id;
            set {
                if (id != value) {
                    id = value;
                    PropertyHasChanged(nameof(id));
                }
            }
        }

        public string Username {
            get => username;
            set {
                if (null == value) {
                    value = string.Empty;
                }

                if (username != value) {
                    username = value;
                    PropertyHasChanged(nameof(id));
                }
            }
        }

        public string Hash {
            get => hash;
            set {
                if (null == value) {
                    value = string.Empty;
                }

                if (hash != value) {
                    hash = value;
                    PropertyHasChanged(nameof(id));
                }
            }
        }

        public string Salt {
            get => salt;
            set {
                if (null == value) {
                    value = string.Empty;
                }

                if (salt != value) {
                    salt = value;
                    PropertyHasChanged(nameof(id));
                }
            }
        }

        public string Email {
            get => email;
            set {
                if (null == value) {
                    value = string.Empty;
                }

                if (email != value) {
                    email = value;
                    PropertyHasChanged(nameof(id));
                }
            }
        }

        public int Skulls {
            get => skulls;
            set {
                if(skulls != value) {
                    skulls = value;
                    PropertyHasChanged(nameof(skulls));
                }
            }
        }

        public int Souls {
            get => souls;
            set {
                if(souls != value) {
                    souls = value;
                    PropertyHasChanged(nameof(souls));
                }
            }
        }

        public List<CardsOwnedByPlayer> PlayerCards {
            get {
                EnsurePlayerCardsListExists();
                return playerCards;
            }
        }

        public List<Deck> Decks {
            get {
                EnsureDeckListExists();
                return decks;
            }
        }

        public static User NullValue { get; } = new User();

        #endregion

        #region Constructors

        public User() { }

        public User(long id, string username, string hash, string salt, string email, int heads, int skulls) {
            this.id = id;
            this.username = username;
            this.hash = hash;
            this.salt = salt;
            this.email = email;
            this.souls = heads;
            this.skulls = skulls;
            base.MarkOld();
        }

        #endregion

        #region Methods

        private void EnsurePlayerCardsListExists() {
            if (playerCards == null) {
                playerCards = (IsNew || id == 0) ? new List<CardsOwnedByPlayer>() : UserMapper.LoadCardsOwnedByPlayer(this).ToList();
            }
        }

        private void EnsureDeckListExists() {
            if (decks == null) {
                decks = (IsNew || id == 0) ? new List<Deck>() : UserMapper.LoadDecks(this).ToList();
            }
        }

        public override bool Equals(object obj) {
            if (obj != null && obj is User other) {
                return GetHashCode().Equals(other.GetHashCode()) && Decks.SequenceEqual(other.Decks) &&
                       PlayerCards.SequenceEqual(other.PlayerCards);
            }

            return false;
        }

        public override int GetHashCode() {
            return ToString().GetHashCode();
        }

        public override string ToString() {
            return string.Format(CultureInfo.CurrentCulture, $"{GetType()}: {username} ({id})");
        }

        #endregion
    }
}