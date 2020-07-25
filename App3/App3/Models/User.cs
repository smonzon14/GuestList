using App3.Data;
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
        public string uid { get; set; }
        public string bio { get; set; }
        public int status { get; set; }
        public string name { get; set; }
        public string email { get; set; }
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
            Debug.WriteLine("status: " + status.ToString());
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
                    var currentUser = App.UserDatabase.GetUser();
                    if (uid != null && currentUser != null && currentUser.uid != null && !uid.Equals(currentUser.uid))
                    {
                        if (FirebaseHelper.AddFriend(currentUser.uid, uid).Result)
                        {
                            friendStatus = 3;
                            App.UserFriends.Add(this);
                        }
                        else friendStatus = 2;
                    }
                });
                else acceptRequestCommand = null;
            } 
        }
        public bool requestRecieved { get { return friendStatus == 2; } }
        
        
    }
}
