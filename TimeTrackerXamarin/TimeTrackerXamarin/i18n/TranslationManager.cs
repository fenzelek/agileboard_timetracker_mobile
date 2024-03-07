using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace TimeTrackerXamarin.i18n
{
    public class TranslationManager : ITranslationManager
    {
        public Language Language { get; private set; }

        private dynamic langDynamic;
        public string Translate(string key)
        {
            try
            {
                var current = langDynamic;
                foreach (var s in key.Split('.'))
                {
                    current = current[s];
                }

                return current;
            }catch(Exception err)
            {
                return key;
            }
        }

        public async Task SetLanguage(Language language)
        {
            var fileName = $"lang-{language.Culture.Name}.json";
            using (var stream = await FileSystem.OpenAppPackageFileAsync(fileName))
            {
                using (var reader = new StreamReader(stream))
                {
                    var json = reader.ReadToEnd();
                    langDynamic = JsonConvert.DeserializeObject<dynamic>(json);
                }
            }

            Language = language;
        }
    }
}