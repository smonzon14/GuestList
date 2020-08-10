using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace App3.Models
{
    public class Comment : INotifyPropertyChanged
    {
        public ICommand LikeCommand { get; set; } = new Command<Comment>((Comment item) =>
        {
            item.likes += item.Liked ? -1 : 1;
            item.Liked = !item.Liked;
        });

        public string cid;
        public string name { get; set; }
        public string uid { get; set; }
        public string message { get; set; }
        public int likes { get; set; }

        private bool liked = false;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public bool Liked
        {
            get { return liked; }
            set
            {
                liked = value;
                OnPropertyChanged("Liked");
                OnPropertyChanged("likes");
            }
        }
    }
}
