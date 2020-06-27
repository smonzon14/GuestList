using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using App3.Data;
using System.Diagnostics;
using System.ComponentModel;

namespace App3.Models
{
    public class Post : INotifyPropertyChanged
    {
        public ICommand LikeCommand { get; set; } = new Command<Post>((Post item) =>
        {
            if (item.liked)
            {
                item.likes--;
                FirebaseHelper.RemoveLikeFromPost(item, App.UserDatabase.GetUser().uid);
            }
            else
            {
                item.likes++;
                FirebaseHelper.AddLikeToPost(item, App.UserDatabase.GetUser().uid);
            }
            item.Liked = !item.Liked;
        });
        private bool liked = false;
        public bool Liked {
            get { return liked; }
            set
            {
                liked = value;
                OnPropertyChanged("Liked");

                OnPropertyChanged("likes");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public string uid;
        public string pid;

        

        public string name { get; set; }
        public string message { get; set; }
        public int comments { get; set; }
        public int likes { get; set; }
    }
}
