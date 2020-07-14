using App3.Data;
using App3.Models;
using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App3.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewPostPage : ContentPage
    {
        public ICommand ToolbarRightCommand { get; private set; }
        public string ToolbarRightSource { get; private set; }

        public ICommand ToolbarLeftCommand { get; private set; }
        public string ToolbarLeftSource { get; private set; }
        public NewPostPage()
        {
            InitializeComponent();
        }

        private async void PostButton_Clicked(object sender, EventArgs e)
        {
            if (message.Text == null || message.Text.Length < 1)
            {
                await DisplayAlert("Can't post an empty message!", null, "Ok");
                return;
            }
            var user = App.UserDatabase.GetUser();
            var post = new Post
            {
                message = message.Text,
                uid = user.uid,
                name = user.name,
                comments = 0,
                likes = 0
            };
            FirebaseHelper.CreatePost(post, user.uid);
            await Navigation.PopModalAsync();
        }

        private void CancelButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}