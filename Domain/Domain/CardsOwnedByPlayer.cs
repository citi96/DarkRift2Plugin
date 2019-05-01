using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Domain {
    [Serializable]
    public class CardsOwnedByPlayer : DomainBase {
        #region Private Field

        private long owner;
        private long cardId;
        private int amount;

        #endregion

        #region Properties

        public long Owner {
            get => owner;
            set {
                if (owner != value) {
                    owner = value;
                    PropertyHasChanged(nameof(owner));
                }
            }
        }

        public long CardId {
            get => cardId;
            set {
                if (cardId != value) {
                    cardId = value;
                    PropertyHasChanged(nameof(cardId));
                }
            }
        }

        public int Amount {
            get => amount;
            set {
                if (amount != value) {
                    amount = value;
                    PropertyHasChanged(nameof(amount));
                }
            }
        }

        public static CardsOwnedByPlayer NullValue { get; } = new CardsOwnedByPlayer();

        #endregion

        #region Constructors

        public CardsOwnedByPlayer() { }

        public CardsOwnedByPlayer(long owner, long cardId, int amount) {
            this.owner = owner;
            this.cardId = cardId;
            this.amount = amount;
            base.MarkOld();
        }

        #endregion

        #region Methods

        public override bool Equals(object obj) {
            if (obj != null && obj is User other) {
                return GetHashCode().Equals(other.GetHashCode());
            }

            return false;
        }

        public override int GetHashCode() {
            return ToString().GetHashCode();
        }

        public override string ToString() {
            return string.Format(CultureInfo.CurrentCulture, $"{GetType()}: {owner} -{cardId}");
        }

        #endregion
    }
}