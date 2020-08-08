using App3.Models;
using Xamarin.Forms;

namespace App3.Converters
{
    public class FeedDataTemplateSelector : DataTemplateSelector
    {
        public static string uid = App.UserDatabase.GetUser().uid;
        public DataTemplate MyTemplate { get; set; }
        public DataTemplate InviteTemplate { get; set; }
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is Party p && p.Thrower.uid == uid)
            {
                return MyTemplate;
            }
            else
            {
                return InviteTemplate;
            }
        }
    }
}
