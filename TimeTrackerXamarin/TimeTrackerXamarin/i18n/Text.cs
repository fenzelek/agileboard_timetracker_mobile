using System;
using Prism.Ioc;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TimeTrackerXamarin.i18n
{
    [ContentProperty("Key")]
    public class Text : IMarkupExtension<BindingBase>
    {
        public string Key { get; set; }

        public BindingBase ProvideValue(IServiceProvider serviceProvider)
        {
            var actualKey = Key.Replace(".", ":");
            var binding = new Binding
            {
                Mode = BindingMode.OneWay,
                Path = $"[{actualKey}]",
                Source = TextBindings.Instance
            };
            return binding;
        }

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
        {
            return ProvideValue(serviceProvider);
        }
    }
}