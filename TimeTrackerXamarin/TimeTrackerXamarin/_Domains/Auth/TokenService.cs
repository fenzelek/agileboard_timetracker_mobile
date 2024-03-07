using System;
using System.Collections.Generic;
using System.Text;
using Flurl.Http;
using Newtonsoft.Json;
using TimeTrackerXamarin._Domains.Auth.Dto;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin.Config;
using Xamarin.Essentials;
using Xamarin.Essentials.Interfaces;

namespace TimeTrackerXamarin._Domains.Auth
{
    public class TokenService: ITokenService
    {

        private readonly IFlurlClient client;
        private readonly IPreferences preferences;

        public TokenService(IFlurlClient client, IPreferences preferences)
        {
            this.client = client;
            this.preferences = preferences;
        }

        public string Get()
        {
            if (preferences.ContainsKey("token") && preferences.ContainsKey("token_time"))
            {
                return preferences.Get("token", "");
            }
            return "";
        }

        public void Set(string token)
        {
            var split = token.Split('.');
            if (split.Length <= 1) throw new Exception("Token can't be decrypt");
            var base64Payload = split[1];
            var payloadJson = Base64DecodeToString(base64Payload);
            var payload = JsonConvert.DeserializeObject<Dictionary<string, string>>(payloadJson);
            var exp = payload["exp"];
            preferences.Set("token_time", exp);
            preferences.Set("token", token);
        }

        public void Remove()
        {
            preferences.Remove("token_time");
            preferences.Remove("token");
        }

        private static string Base64DecodeToString(string toDecode)
        {
            var decodePrepped = toDecode.Replace("-", "+").Replace("_", "/");

            switch (decodePrepped.Length % 4)
            {
                case 0:
                    break;
                case 2:
                    decodePrepped += "==";
                    break;
                case 3:
                    decodePrepped += "=";
                    break;
                default:
                    throw new Exception("Not a legal base64 string!");
            }

            var data = Convert.FromBase64String(decodePrepped);
            return Encoding.UTF8.GetString(data);
        }


        public bool Refresh()
        {
            //todo brak strategii refreshu token'a
            return true;
        }
    }
}