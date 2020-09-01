using App3.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace App3.Models
{

    public class Party : INotifyPropertyChanged
    {
        
        public ICommand LikeCommand { get; set; } = new Command<Party>((Party item) =>
        {
            item.likes += item.Liked ? -1 : 1;
            item.Liked = !item.Liked;
        });

        public ICommand GoCommand { get; set; } = new Command<Party>((Party item) =>
        {
            if (item.Going)
            {
                item.Going = false;
                FirebaseHelper.UndoGoToParty(item.pid);
            }
            else
            {
                item.Going = true;
                FirebaseHelper.GoToParty(item.pid);
            }
        });
        public Party()
        {

        }
        private bool liked = false;
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
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public string pid { get; set; }
        public string name { get; set; }
        public int likes { get; set; }
        public int comments { get; set; }
        public string description { get; set; }
        private bool going { get; set; } = false;
        public bool Going { get { return going; } set { going = value; OnPropertyChanged("Going"); } } 
        public User Thrower { get; set; }
        public DateTime time { get; set; }
        public DateTime posted { get; set; }
        public string address { get; set; }
        public Position geoPosition { get; set; }
        public bool img { get; set; }
        

        public static List<string> GetRandomHexColor()
        {
            var rand = new Random();
            int hue = rand.Next(0, 255);
            Color c1 = Color.FromHsla(
                (hue / 255.0f),
                0.7f,
                0.3f);

            hue -= 10;
            if (hue < 0) hue += 255;

            Color c2 = Color.FromHsla(
                (hue / 255.0f),
                0.7f,
                0.3f, 0.75);
            return new List<string> { c1.ToHex(), c2.ToHex() };
        }
        public string primaryHexColor
        {
            get
            {
                if (primaryHexColor == null)
                {
                    var colors = GetRandomHexColor();
                    primaryHexColor = colors[0];
                    secondaryHexColor = colors[1];
                }
                return primaryHexColor;
            }
            set
            {
                primaryHexColor = value;
            }
        }
        private string imageSource { get; set; }
        public string ImageSource { get { return imageSource; } 
            set {
                imageSource = value;
                OnPropertyChanged("ImageSource");
            } 
        }
        public async void updateImageSource()
        {
            
            ImageSource = await FirebaseHelper.GetPostedImageURL(pid);
            if (ImageSource == null) img = false;
        }
        public string secondaryHexColor { get; set; }

    }
}
