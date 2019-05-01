using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace GameCommon {
    public class MessageSerializerService {
        public static object SerializeObjectOfType<T>(T objectToSerialize) where T : class {
            object returnValue = null;

            returnValue = SerializeJson<T>(objectToSerialize);
            //returnValue = SerializeBson<T>(objectToSerialize);
            return returnValue;
        }

        public static T DeserializeObjectOfType<T>(object objectToDeserialize) where T : class {
            T returnValue = null;

            returnValue = DeserializeJson<T>(objectToDeserialize);

            //returnValue = DeserializeBson<T>(objectToDeserialize);

            return returnValue;
        }

        #region Json

        private static object SerializeJson<T>(T objectToSerialize) where T : class {
            return JsonConvert.SerializeObject(objectToSerialize);
        }

        private static T DeserializeJson<T>(object objectToDeserialize) where T : class {
            return JsonConvert.DeserializeObject<T>((string) objectToDeserialize);
        }

        #endregion

        #region Bson

        private static object SerializeBson<T>(T objectToSerialize) where T : class {
            var ms = new MemoryStream();

            using (var writer = new BsonDataWriter(ms)) {
                var serializer = new JsonSerializer();
                serializer.Serialize(writer, objectToSerialize);
            }

            return Convert.ToBase64String(ms.ToArray());
        }

        private static T DeserializeBson<T>(object objectToDeserialize) where T : class {
            var data = Convert.FromBase64String((string) objectToDeserialize);
            var ms = new MemoryStream(data);

            using (var reader = new BsonDataReader(ms)) {
                var serializer = new JsonSerializer();
                return serializer.Deserialize<T>(reader);
            }
        }

        #endregion
    }
}