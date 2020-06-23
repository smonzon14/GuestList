using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using App3.Models;
using App3.Data;

namespace App3.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PostViewPage : ContentPage
    {
        public System.Windows.Input.ICommand ToolbarLeftCommand { get; private set; }
        public string ToolbarLeftSource { get; private set; }
        private Post post;
        private List<Comment> comments;
        public PostViewPage(Post post)
        {
            this.post = post;
            BindingContext = post;
            NavigationPage.SetHasNavigationBar(this, false);
            ToolbarLeftCommand = new Command(() =>
            {
                Navigation.PopAsync();
            });
            ToolbarLeftSource = "button_back";
            InitializeComponent();
            populateComments();
        }
        private async void populateComments()
        {
            comments = await FirebaseHelper.GetCommentsForPost(post.pid);
            commentsListView.ItemsSource = comments;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            if (commentEditor.Text == null || commentEditor.Text.Length < 1) return;
            var message = commentEditor.Text;

            commentEditor.Text = "";
            var newComment = await FirebaseHelper.AddCommentToPost(new Comment
            {
                message = message,
                name = App.UserDatabase.GetUser().name
            }, post.pid);

            comments.Insert(0, newComment);
            commentsListView.ItemsSource = null;
            commentsListView.ItemsSource = comments;

            await progressBar.ProgressTo(1.0, 250, Easing.SinInOut);

            await Task.Delay(200);
            progressBar.Progress = 0;
        }

        private void RefreshView_Refreshing(object sender, EventArgs e)
        {
            populateComments();
            var refesh = sender as RefreshView;
            refesh.IsRefreshing = false;
        }
    }
}