using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App3
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Host : ContentPage
    {
        TabbedPage1 parent;
        public Host(TabbedPage1 parent)
        {
            this.parent = parent;
            InitializeComponent();
        }
        async public void OnSettingsButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Settings(this.parent));
        }
    }
}