using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Diagnostics;
using System.ComponentModel;
using App3.Data;

namespace App3.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MusicPage : ContentPage
    {
        private static List<MusicGenre> MusicGenres = new List<MusicGenre> {
            new MusicGenre { Genre = "Pop" },
            new MusicGenre { Genre = "Rock" },
            new MusicGenre { Genre = "Hip Hop" },
            new MusicGenre { Genre = "Rap" },
            new MusicGenre { Genre = "EDM" },
            new MusicGenre { Genre = "Metal" },
            new MusicGenre { Genre = "Dubstep" },
            new MusicGenre { Genre = "Latin" }};

        internal class MusicGenre :INotifyPropertyChanged{
            public string Genre { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;
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
            private void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public MusicPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            genreListView.ItemsSource = MusicGenres;
        }
        public async void onSubmit(object sender, EventArgs e)
        {
            byte val = 0;
            foreach(var genre in MusicGenres)
            {
                Debug.WriteLine(genre.Genre + ": " + genre.Liked.ToString());
                val <<= 1;
                if (genre.Liked) val |= 1;
            }
            FirebaseHelper.UpdateMusicPreferences(val);
            
            await Navigation.PopModalAsync();
        }

        private void ListView_Refreshing(object sender, EventArgs e)
        {
            genreListView.IsRefreshing = false;
        }

        private void genreListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var tg = (e.Item as MusicGenre);
            tg.Liked = !tg.Liked;
        }
    }
}