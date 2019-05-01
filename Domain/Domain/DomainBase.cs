using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Domain.Domain {
    public class DomainBase : IProcessDirty {
        private bool isObjectNew = true;
        private bool isObjectDirty = true;
        private bool isObjectDeleted = false;

        #region IProcessDirty memebers

        [Browsable(false)]
        public bool IsNew {
            get => isObjectNew;
            set => isObjectNew = value; //Only used during deserialization - must be public
        }

        [Browsable(false)]
        public bool IsDirty {
            get => isObjectDirty;
            set => isObjectDirty = value; //Only used during deserialization - must be public
        }

        [Browsable(false)]
        public bool IsDeleted {
            get => isObjectDeleted;
            set => isObjectDeleted = value; //Only used during deserialization - must be public
        }

        #endregion

        [NonSerialized] private PropertyChangedEventHandler _nonSerializableHandlers;

        private PropertyChangedEventHandler _serializableHandlers;

        // Usually some validation goes here.
        [Browsable(false), XmlIgnore] public virtual bool IsSavable => isObjectDirty;

        /// <inheritdoc />
        ///  <summary>
        /// Pattern from CSLA.Net - a Domain Driven Design Pattern based on BindableBase - cslanet.com.
        /// Necessary to make serialization work properly and more importantly safely.
        ///  </summary>
        public event PropertyChangedEventHandler PropertyChanged {
            add {
                if (value.Method.DeclaringType != null &&
                    value.Method.IsPublic && (value.Method.DeclaringType.IsSerializable || value.Method.IsStatic)) {
                    _serializableHandlers =
                        (PropertyChangedEventHandler) Delegate.Combine(_serializableHandlers, value);
                } else {
                    _nonSerializableHandlers =
                        (PropertyChangedEventHandler) Delegate.Combine(_nonSerializableHandlers, value);
                }
            }

            remove {
                if (value.Method.DeclaringType != null &&
                    value.Method.IsPublic && (value.Method.DeclaringType.IsSerializable || value.Method.IsStatic)) {
                    _serializableHandlers =
                        (PropertyChangedEventHandler) Delegate.Remove(_serializableHandlers, value);
                } else {
                    _nonSerializableHandlers =
                        (PropertyChangedEventHandler) Delegate.Remove(_nonSerializableHandlers, value);
                }
            }
        }

        /// <summary>
        /// Automatically called by markDirty. Refresh all properties (useful in application that need to refresh data).
        /// </summary>
        protected virtual void OnUnknownPropertyChange() {
            OnPropertyChange(string.Empty);
        }

        protected virtual void OnPropertyChange(string propertyName) {
            _nonSerializableHandlers?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            _serializableHandlers?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Used by the constructor to denote a brand new object that is not stored in the database and ensure it isn't being marked fore deletion.
        /// </summary>
        protected virtual void MarkNew() {
            isObjectNew = true;
            isObjectDeleted = false;
            MarkDirty();
        }

        /// <summary>
        /// Used by fetch to denote an object that already exists and has been pulled from the database;
        /// </summary>
        protected virtual void MarkOld() {
            isObjectNew = false;
            MarkClean();
        }

        protected void MarkClean() {
            isObjectDirty = false;
        }

        protected void MarkDeleted() {
            isObjectDeleted = true;
            MarkDirty();
        }

        /// <summary>
        /// Call any time data changes to denote that the object needs to be saved.
        /// </summary>
        protected void MarkDirty(bool suppressEvent = false) {
            isObjectDirty = true;
            if (!suppressEvent) {
                // Force properties to refresh.
                OnUnknownPropertyChange();
            }
        }

        protected void PropertyHasChanged() {
            PropertyHasChanged(new StackTrace().GetFrame(1).GetMethod().Name.Substring(4));
        }

        protected virtual void PropertyHasChanged(string propertyName) {
            MarkDirty(true);
            OnPropertyChange(propertyName);
        }

        public virtual void Delete() {
            MarkDeleted();
        }
    }
}