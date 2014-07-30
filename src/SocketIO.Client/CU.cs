using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SocketIO.Client
{
    static class CU
    {
        static object _obj4Lock = new object();
        internal static void Write2File(string fileFullPath, string content)
        {
            lock (_obj4Lock)
            {
                if (!File.Exists(fileFullPath))
                    using (File.Create(fileFullPath)) { }

                using (FileStream fs = new FileStream(fileFullPath, FileMode.Append))
                using (StreamWriter sw = new StreamWriter(fs))
                    sw.WriteLine(content);
            }
        }

        internal static void Log(string format, params object[] args)
        {
            string sFileName = string.Format("{0:yyyy-MM-dd}.txt", DateTime.Now);
            Write2File(sFileName, string.Format(format, args));
        }

        static JsonSerializerSettings _setting = new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() };

        public static string JsonSerialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.None, _setting);
        }

        public static T JsonDeserialize<T>(string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString, _setting);
        }

        /// <summary>
        /// 【闻祖东 2012-2-18-233326】将与枚举值字符串相同的字符串转换为相应的枚举值，不区分大小写。
        /// 注意泛型T一定需要是一个枚举值类型，否则会在运行时抛出自定义异常。
        /// </summary>
        /// <typeparam name="T">实际的枚举类型</typeparam>
        /// <param name="strValue">字符串的值</param>
        /// <param name="defaultValue">无法匹配时所取的默认值</param>
        /// <returns>匹配的枚举值</returns>
        public static T Convert2Enum<T>(object obj, T defaultValue) where T : struct, IComparable, IFormattable, IConvertible
        {
            ///【闻祖东 2012-2-18-233447】这里实在是没有办法在声明泛型T的时候约束他一定要继承自System.Enum，至少在.NET 4.0里面都还是不行的，不只是CRC遇到这个问题，
            ///网上很多帖子也是关于这个的，但是只能在运行时抛出异常，而不能在编译时控制编译。
            ///http://stackoverflow.com/questions/79126/create-generic-method-constraining-t-to-an-enum
            ///http://code.google.com/p/unconstrained-melody/downloads/list

            if (typeof(T).IsEnum)
            {
                if (obj == null)
                {
                    return defaultValue;
                }

                T t = defaultValue;
                string val = obj.ToString();

                return Enum.TryParse(val, true, out t) && Enum.IsDefined(typeof(T), t)
                    ? t
                    : defaultValue;
            }

            throw new Exception("【闻祖东 2014-1-26-115324】运行时异常，输入泛型类型<T>类型应该是一个枚举类型。");
        }
    }
}
