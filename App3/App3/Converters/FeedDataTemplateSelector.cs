using App3.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace App3.Converters
{
    public class FeedDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate PostTemplate { get; set; }
        public DataTemplate PartyTemplate { get; set; }
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is Post) return PostTemplate;
            return PartyTemplate;
        }
    }
}
