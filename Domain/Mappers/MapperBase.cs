using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Domain.Domain;

namespace Domain.Mappers {
    public abstract class MapperBase<T> where T : DomainBase {
        public XDocument ConvertToXML(string rootElementName, string data) {
            var rootElement = new XElement(rootElementName);

            if (!string.IsNullOrEmpty(data)) {
                var xElement = XDocument.Parse(data).Root;
                if (xElement != null) {
                    rootElement.Add(xElement.Nodes());
                }
            }

            return new XDocument(rootElement);
        }

        public void Delete(long id) {
            DeleteNow(id);
        }

        public T Save(T obj) {
            if (obj.Equals(null)) {
                throw new ArgumentNullException(nameof(obj));
            }

            if (!obj.IsSavable) {
                // throw new InvalidOperationException("Operation cannot be completed on object in the current state");
            }

            if (obj.IsNew) {
                obj = this.Insert(obj);
            } else if (obj.IsDeleted) {
                obj = Delete(obj);
            } else if (obj.IsDirty) {
                obj = Update(obj);
            }

            return obj;
        }

        public T Load(long id) {
            return Fetch(id);
        }

        protected abstract T Delete(T domainObject);

        protected abstract void DeleteNow(long id);

        protected abstract T Insert(T domainObject);

        /// <summary>
        /// Updates the object to the actual database, until update is called, the entity might have unsaved changes, denoted by IsDirty flag.
        /// </summary>
        /// <param name="domainObject"></param>
        /// <returns></returns>
        protected abstract T Update(T domainObject);

        /// <summary>
        /// Gets all objects of this type and return a list.
        /// </summary>
        /// <returns></returns>
        protected abstract IList<T> Fetch();

        protected abstract T Fetch(long id);

        /// <summary>
        /// Maps an entity over to an actual domain object. Provides separation from EF6 vs Hibernate classes.
        /// </summary>
        /// <param name="domainObject"></param>
        /// <param name="entity"></param>
        protected abstract void Map(T domainObject, object entity);
    }
}