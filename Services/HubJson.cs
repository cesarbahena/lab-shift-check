using System;
using Newtonsoft.Json;

namespace ShiftCheck.Services
{
    public static class HubJson
    {
        private const int MaxResponseLength = 1000000;
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MaxDepth = 32
        };

        public static T Read<T>(string json)
        {
            if (json == null || json.Length > MaxResponseLength)
                throw new InvalidOperationException("La respuesta de Hub es demasiado grande.");

            return JsonConvert.DeserializeObject<T>(json, Settings);
        }

        public static string Write<T>(T value)
        {
            return JsonConvert.SerializeObject(value);
        }
    }
}
