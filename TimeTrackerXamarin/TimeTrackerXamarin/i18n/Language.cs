using System.Collections.Generic;
using System.Globalization;
using TimeTrackerXamarin._UseCases.Contracts;

namespace TimeTrackerXamarin.i18n
{
    public sealed class Language
    {

        public static readonly Language Polish = new Language
        {
            Name = "Polski",
            Culture = new CultureInfo("pl-PL"),
            Flag = new Emoji(0x1F1F5, 0x1F1F1)
        };
        public static readonly Language English = new Language
        {
            Name = "English",
            Culture = new CultureInfo("en-US"),
            Flag = new Emoji(0x1F1EC, 0x1F1E7)
        };
        public static readonly Language Ukrainian = new Language
        {
            Name = "українська",
            Culture = new CultureInfo("uk-UA"),
            Flag = new Emoji(0x1F1FA, 0x1F1E6)
        };

        public static readonly IEnumerable<Language> AllLanguages = new List<Language>
        {
            Polish,English,Ukrainian
        };

        public string Name { get; private set; }
        public CultureInfo Culture { get; private set; }
        
        public Emoji Flag { get; private set; }
        
        private Language() {}
    }
}
