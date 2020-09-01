using App3.Data;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using Xamarin.Forms;

namespace App3.Models
{
    public class User : INotifyPropertyChanged
    {
        public ICommand acceptRequestCommand { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        public User() { }
        public byte music { get; set; }
        public string uid { get; set; }
        public string bio { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public int age { get {
                if (birthday == null) return -1;
                int a = DateTime.Now.Year - birthday.Year;
                if (DateTime.Now.DayOfYear < birthday.DayOfYear) a -= 1;
                return a;
            }
        }
        public DateTime birthday { get; set; }
        public int gender { get; set; }
        private string profileImageSource { get; set; }
        public string ProfileImageSource { get { return profileImageSource == null ? "Profile" : profileImageSource; } set { profileImageSource = value; OnPropertyChanged("ProfileImageSource"); } }
        public async void updateProfileImageSource()
        {

            ProfileImageSource = await FirebaseHelper.GetProfileImageURL(uid);
        }
        public void printUser()
        {
            Debug.WriteLine("id: " + uid);
            Debug.WriteLine("bio: " + bio);
            Debug.WriteLine("name: " + name);
            Debug.WriteLine("email: " + email);
        }
        private int friendStatus;
        public int FriendStatus {
            get {
                return friendStatus;
            }
            set {
                friendStatus = value;
                OnPropertyChanged("FriendStatus");
                if (requestRecieved) acceptRequestCommand = new Command(() => {
                    if (FirebaseHelper.AddFriend(uid).Result)
                    {
                        friendStatus = 3;
                        if(!App.UserFriends.Exists((item) => { return item.uid == uid; }))
                        {
                            App.UserFriends.Add(this);
                        }
                    }
                    else friendStatus = 2;
                    
                });
                else acceptRequestCommand = null;
            } 
        }
        public bool requestRecieved { get { return friendStatus == 2; } }
        
        
    }
}
