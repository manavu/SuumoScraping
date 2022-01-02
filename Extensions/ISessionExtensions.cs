namespace SuumoScraping.Extensions
{
    using System;
    using Newtonsoft.Json;
    using Microsoft.AspNetCore.Http;

    public static class ISessionExtensions
    {
        public static void Put<T>(this ISession session, string key, T value) where T : class
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T Get<T>(this ISession session, string key) where T : class
        {
            session.TryGetValue(key, out var obj);
            return obj == null ? default(T) : JsonConvert.DeserializeObject<T>(obj.ToString());
        }
    }
}
