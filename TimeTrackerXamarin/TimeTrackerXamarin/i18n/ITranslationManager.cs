using System.Threading.Tasks;

namespace TimeTrackerXamarin.i18n
{
    public interface ITranslationManager
    {

        Language Language { get; }
        
        string Translate(string key);

        Task SetLanguage(Language language);

    }
}