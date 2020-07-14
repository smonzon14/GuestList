using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace App3.Models
{

    public class Party : INotifyPropertyChanged
    {
        public ICommand LikeCommand { get; set; } = new Command<object>((object item) =>
        {
            var obj = item as Party;
            if (obj.liked)
                obj.likes--;
            else obj.likes++;
            obj.Liked = !obj.Liked;
        });
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
        public bool going { get; set; }
        public string thrower { get; set; }
        public string throwerid { get; set; }
        public DateTime time { get; set; }
        public string address { get; set; }
        public Position geoPosition { get; set; }


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
        public string secondaryHexColor { get; set; }

    }
}
