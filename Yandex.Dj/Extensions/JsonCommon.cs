using System;
using System.IO;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Yandex.Dj.Extensions
{
    /// <summary>
    /// Предоставляет набор методов расширения для работы с JSON
    /// </summary>
    public static class JsonCommon
    {
        /// <summary>
        /// Преобразование в .Net-объект
        /// </summary>
        public static object ToNetObject(this JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    return ((JObject)token).Properties()
                        .ToDictionary(prop => prop.Name, prop => prop.Value.ToNetObject());
                case JTokenType.Array:
                    return token.Values().Select(ToNetObject).ToList();
                default:
                    return token.ToObject<object>();
            }
        }

        /// <summary>
        /// Проверка на пустоту
        /// </summary>
        public static bool IsNullOrEmpty(this JToken token)
        {
            return token == null ||
                token.Type == JTokenType.Array && !token.HasValues ||
                token.Type == JTokenType.Object && !token.HasValues ||
                token.Type == JTokenType.String && token.ToString() == String.Empty ||
                token.Type == JTokenType.Null;
        }

        /// <summary>
        /// Загрузка из файла
        /// </summary>
        public static JObject Load(string fileName)
        {
            FileStream fs = null;
            StreamReader sr = null;

            try
            {
                fs = new FileStream(fileName, FileMode.Open);
                sr = new StreamReader(fs);
                return JObject.Parse(sr.ReadToEnd());
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                sr?.Close();
                fs?.Close();
            }
        }

        public static T Load<T>(string fileName)
        {
            FileStream fs = null;
            StreamReader sr = null;

            try
            {
                fs = new FileStream(fileName, FileMode.Open);
                sr = new StreamReader(fs);
                return JsonConvert.DeserializeObject<T>(sr.ReadToEnd());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return default(T);
            }
            finally
            {
                sr?.Close();
                fs?.Close();
            }
        }
    }
}
