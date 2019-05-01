using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Domain {
    [Serializable]
    public class Deck : DomainBase {
        #region Private Field

        private long id;
        private long owner;
        private string name;
        private long _class;

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

        public long Owner {
            get => owner;
            set {
                if (owner != value) {
                    owner = value;
                    PropertyHasChanged(nameof(owner));
                }
            }
        }

        public string Name {
            get => name;
            set {
                if(null == value) {
                    value = string.Empty;
                }

                if(name != value) {
                    name = value;
                    PropertyHasChanged(nameof(name));
                }
            }
        }

        public long Class {
            get => _class;
            set {
                if(_class != value) {
                    _class = value;
                    PropertyHasChanged(nameof(_class));
                }
            }
        }

        public static Deck NullValue { get; } = new Deck();

        #endregion

        #region Constructors

        public Deck() { }

        public Deck(long id, long owner,  string name, long _class) {
            this.id = id;
            this.owner = owner;
            this.name = name;
            this._class = _class;
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
            return string.Format(CultureInfo.CurrentCulture, $"{GetType()}: {name} ({id})");
        }

        #endregion
    }
}