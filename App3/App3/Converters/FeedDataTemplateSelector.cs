using App3.Models;
using Xamarin.Forms;

namespace App3.Converters
{
    public class FeedDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate PostTemplate { get; set; }
        public DataTemplate SecondaryTemplate { get; set; }
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is Post) return PostTemplate;
            return SecondaryTemplate;
        }
    }
}
