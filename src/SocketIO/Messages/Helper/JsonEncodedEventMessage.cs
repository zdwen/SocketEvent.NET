using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace SocketIOClient.Messages
{
    /// <summary>
    /// TODO【闻祖东 2014-7-25-143652】这个地方的json序列化在处理枚举、事件、ObjectID类型的时候可能会存在一定的问题。
    /// </summary>
    public class JsonEncodedEventMessage
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "args")]
        public dynamic[] Args { get; set; }

        public JsonEncodedEventMessage() { }

        public JsonEncodedEventMessage(string name, object payload) : this(name, new[] { payload }) { }

        public JsonEncodedEventMessage(string name, object[] payloads)
        {
            Name = name;
            Args = payloads;
        }

        public T GetFirstArgAs<T>()
        {
            var firstArg = Args.FirstOrDefault();

            return firstArg == null
                ? default(T)
                : JsonConvert.DeserializeObject<T>(firstArg.ToString());
        }
        public IEnumerable<T> GetArgsAs<T>()
        {
            List<T> items = new List<T>();
            foreach (var i in this.Args)
                items.Add(JsonConvert.DeserializeObject<T>(i.ToString(Formatting.None)));

            return items.AsEnumerable();
        }

        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this, Formatting.None);
        }

        public static JsonEncodedEventMessage Deserialize(string jsonString)
        {
            JsonEncodedEventMessage msg = null;
            try
            {
                msg = JsonConvert.DeserializeObject<JsonEncodedEventMessage>(jsonString);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
            return msg;
        }
    }
}
