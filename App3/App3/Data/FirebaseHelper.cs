
using App3.Models;
using Firebase.Database;
using Firebase.Database.Offline;
using Firebase.Database.Query;
using Firebase.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace App3.Data
{
    public static class FirebaseHelper
    {

        //Connect app with firebase using API Url  

        static FirebaseClient firebase = new FirebaseClient("https://tribal-dispatch-276722.firebaseio.com/", new FirebaseOptions { OfflineDatabaseFactory = (t,s) => new OfflineDatabase(t,s) });
        static FirebaseStorage storage = new FirebaseStorage("tribal-dispatch-276722.appspot.com");
        
        
        static string UID { 
            get
            {
                return App.UserDatabase.GetUser().uid;
            }
        }
        static ChildQuery CurrentUser = firebase.Child("Users").Child(UID);
        /*
         * User
         */

        internal class UserDataModel
        {
            public string bio { get; set; }
            public string name { get; set; }
            public string email { get; set; }
            public DateTime birthday { get; set; }
            public int gender { get; set; }
            public byte music { get; set; }
        }
        internal class PartyDataModel
        {
            public DateTime posted { get; set; }
            public string name { get; set; }
            public string description { get; set; }
            public string address { get; set; }
            public bool img { get; set; }
            public DateTime time { get; set; }
        }
        internal class CommentDataModel
        {
            public CommentDataModel() { }
            public CommentDataModel(Comment comment)
            {
                cid = comment.cid;
                name = comment.name;
                uid = comment.uid;
                message = comment.message;
                likes = 0;
            }
            public string cid { get; set; }
            public string name { get; set; }
            public string uid { get; set; }
            public string message { get; set; }
            public int likes { get; set; }
        }
        internal class StoryDataModel
        {
            public string location { get; set; }
        }
        public static async Task SetNotificationToken(string token)
        {
            
            await CurrentUser.Child("notificationTokens").Child(token).PutAsync(true).ConfigureAwait(false);
            
        }
        public static async Task<List<User>> FindUsersMatching(string query)
        {
            
            if (query.Length == 0) return new List<User>();
            try
            {
                var task = firebase.Child("Users").OrderBy("name").StartAt(query).EndAt(query + "\uf8ff").LimitToFirst(15).OnceAsync<UserDataModel>().ConfigureAwait(false);

                var possibleUsers = (await task).Select(item => new User
                {
                    uid = item.Key,
                    name = item.Object.name,
                    bio = item.Object.bio,
                    music = item.Object.music,
                    email = item.Object.email,
                    birthday = item.Object.birthday,
                    gender = item.Object.gender
                    
                }).ToList();
                foreach(var user in possibleUsers)
                {
                    user.updateProfileImageSource();
                }
                return possibleUsers;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }

        }

        public static async Task<User> GetUserFromUID(string uid)
        {
            
            try
            {
                Debug.WriteLine("Getting User: " + uid);
                UserDataModel user = await CurrentUser.OnceSingleAsync<UserDataModel>().ConfigureAwait(false);
                
                var u = new User { name = user.name, uid = uid, bio = user.bio, email = user.email, birthday = user.birthday, gender = user.gender, music = user.music};
                u.updateProfileImageSource();
                return u;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }
        public static async Task<bool> AddUser(User user)
        {
            Debug.WriteLine("Adding User...");
            try
            {
                
                var data = new UserDataModel { bio = user.bio, name = user.name, email = user.email, birthday = user.birthday, gender = user.gender, music = user.music};
                await CurrentUser.PutAsync(data).ConfigureAwait(false);
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error:{e}");
                return false;
            }
        }


        /*
         * Friends
         */

        public static async Task<List<User>> GetFriendsList()
        {
            Debug.WriteLine("Getting Friends List...");

            var friendIdList = (await firebase.Child("Friendships").Child(UID).OnceAsync<int>().ConfigureAwait(false));

            List<User> friends = new List<User>();
            foreach (FirebaseObject<int> u in friendIdList)
            {
                Debug.WriteLine("==");
                var f = await GetUserFromUID(u.Key);
                Debug.WriteLine("&&");
                if (f != null)
                {
                    f.uid = u.Key;
                    f.FriendStatus = u.Object;
                    f.updateProfileImageSource();
                    friends.Add(f);
                }

            }
            return friends;
        }

        // returns success boolean
        public static async Task<bool> AddFriend(string friendid)
        {
            if (friendid == null || UID.Equals(friendid)) return false;
            Debug.WriteLine("Adding Friend...");
            try
            {
                await firebase.Child("Friendships").Child(UID).Child(friendid).PutAsync(1).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error adding friend: " + e.Message);
            }

            return true;

        }
        public static async Task<bool> RemoveFriend(string friendid)
        {
            if (friendid == null || UID.Equals(friendid)) return false;
            Debug.WriteLine("Removing Friend...");
            try
            {
                await firebase.Child("Friendships").Child(UID).Child(friendid).DeleteAsync().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error removing friend: " + e.Message);
            }
            return true;
        }
        

        /*
         * Party
         */

        

        public static async Task<bool> CreateParty(Party party, Plugin.Media.Abstractions.MediaFile mediaFile, int exclusivity)
        {
            Debug.WriteLine("Creating Party...");
            try
            {
                var item = await firebase.Child("Parties").Child(UID).PostAsync(new PartyDataModel
                {
                    posted = DateTime.Now,
                    address = party.address,
                    description = party.description,
                    name = party.name,
                    time = party.time,
                    img = mediaFile != null,
                });
                if (item == null)
                {
                    Debug.WriteLine("Party Item is null");
                    return false;
                }
                var storyRef = storage.Child("Parties").Child(item.Key);
                if(mediaFile != null)
                {
                    var url = await storyRef.PutAsync(mediaFile.GetStream());
                    party.ImageSource = url;
                }
                
                
                return true;

            }
            catch (Exception e)
            {
                Debug.WriteLine("Could not Create party: " + e);
                return false;
            }
        }

        public static async Task<List<Party>> GetInvitedParties(List<User> friends)
        {
            Debug.WriteLine("Getting Invites...");
            List<Party> parties = new List<Party>();
            try
            {
                foreach (User friend in friends)
                {
                    parties.AddRange(await GetPartiesThrownByUser(friend).ConfigureAwait(false));
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error Retrieving Invites: " + e.Message);
            }
            
            return parties;
        }

        /*public static async Task<List<Party>> GetPublicPartiesForRegion(double Longitude, double Latitude)
        {

        }*/
        public static async Task<Party> GetParty(string partyId, User thrower)
        {
            Debug.WriteLine("Getting Party...");
            try
            {
                Party party = await firebase.Child("Parties").Child(thrower.uid).Child(partyId).OnceSingleAsync<Party>().ConfigureAwait(false);
                party.Thrower = thrower;
                party.pid = partyId;
                if (party.img) party.updateImageSource();
                return party;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Could Not find party with id (" + partyId + "): " + e);
                return null;
            }
        }

        public static async Task<List<Party>> GetPartiesThrownByUser(User user)
        {
            Debug.WriteLine("Getting Parties...");
            var partiesList = new List<Party>();
            try
            {
                var firebaseObjects = (await firebase.Child("Parties").Child(user.uid).OnceAsync<Party>().ConfigureAwait(false));
                if (firebaseObjects != null)
                {
                    foreach (var obj in firebaseObjects)
                    {
                        var party = obj.Object;
                        party.pid = obj.Key;
                        party.Thrower = user;
                        Debug.WriteLine(party.name + ": " + party.posted.ToString());
                        if(party.img) party.updateImageSource();
                        partiesList.Add(party);
                        
                    }
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine("error: " + e);
            }
            return partiesList;


        }
        /* 
         * Posts
         */
        public static async void CreatePost(Post post)
        {
            try
            {

                var p = await firebase.Child("Posts").Child(UID).PostAsync(post).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

        }
        public static async Task<Post> GetPost(string postid, string userid)
        {
            return await firebase.Child("Posts").Child(userid).Child(postid).OnceSingleAsync<Post>().ConfigureAwait(false);
        }
        public static async Task<List<Post>> GetPostsForUser(string userid)
        {
            var posts = new List<Post>();
            var firebaseObjects = await firebase.Child("Posts").Child(userid).OnceAsync<Post>().ConfigureAwait(false);
            if (firebaseObjects != null)
            {
                foreach (var obj in firebaseObjects)
                {
                    var post = obj.Object;
                    post.pid = obj.Key;
                    posts.Add(post);
                }
            }
            return posts;
        }

        /*
         * Comments
         */
        
        public static async Task<Comment> AddCommentToPost(Comment comment, string pid)
        {
            var data = new CommentDataModel(comment);
            var c = await firebase.Child("Comments").Child(pid).PostAsync(data).ConfigureAwait(false);
            var newComment = c.Object;
            newComment.cid = c.Key;
            return new Comment
            {
                cid = c.Key,
                name = c.Object.name,
                uid = c.Object.uid,
                message = c.Object.message,
                likes = 0
            };
        }
        public static async Task<List<Comment>> GetCommentsForPost(string pid)
        {
            var list = new List<Comment>();

            var firebaseObjects = (await firebase.Child("Comments").Child(pid).OrderBy("likes").OnceAsync<Comment>().ConfigureAwait(false));
            if (firebaseObjects != null)
            {
                foreach (var obj in firebaseObjects)
                {
                    var c = obj.Object;
                    c.cid = obj.Key;
                    list.Add(c);
                }
            }
            return list;
        }



        public static async Task<Comment> AddCommentToParty(Comment comment, string pid)
        {
            var data = new CommentDataModel(comment);
            var c = await firebase.Child("Comments").Child(pid).PostAsync(data).ConfigureAwait(false);
            var newComment = c.Object;
            newComment.cid = c.Key;
            return new Comment
            {
                cid = c.Key,
                name = c.Object.name,
                uid = c.Object.uid,
                message = c.Object.message,
                likes = 0
            };
        }
        public static async Task<List<Comment>> GetCommentsForParty(string pid)
        {
            var list = new List<Comment>();
            
            var firebaseObjects = (await firebase.Child("Comments").Child(pid).OrderBy("likes").OnceAsync<Comment>().ConfigureAwait(false));
            if (firebaseObjects != null)
            {
                foreach (var obj in firebaseObjects)
                {
                    var c = obj.Object;
                    c.cid = obj.Key;
                    list.Add(c);
                }
            }
            return list;
        }

        /*
         * Stories
         */
        
        public static async void PostStory(Story story)
        {
            try
            {
                var firebaseObj = await firebase.Child("Stories").Child(story.uid).PostAsync(new StoryDataModel { location = "HEllo" }).ConfigureAwait(false);
                var key = firebaseObj?.Key;
                var storyRef = storage
                    .Child("Stories")
                    .Child(App.UserDatabase.GetUser().uid)
                    .Child(key);
                
                var url = await storyRef
                    .PutAsync(story.media);
                

                Debug.WriteLine("Story posted: " + url);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Could not post story: " + e.Message);
            }

        }
        public static async Task<string> GetPostedImageURL(string pid)
        {
            string url = null;
            try
            {
                url = await storage.Child("Parties").Child(pid).GetDownloadUrlAsync().ConfigureAwait(false);
            }catch (FirebaseStorageException)
            {
                Debug.WriteLine("URL not found");
            }
            return url;
        }
        public static async Task<string> SetProfileImage(Plugin.Media.Abstractions.MediaFile mediaFile)
        {
            string url = null;
            try
            {
                var storyRef = storage.Child("Profiles").Child(UID);

                if (mediaFile != null)
                {
                    url = await storyRef.PutAsync(mediaFile.GetStream());
                }
            }
            catch (FirebaseStorageException)
            {
                Debug.WriteLine("URL not found");
            }

            return url;
        } 
        public static async Task<string> GetProfileImageURL(string uid)
        {
            string url = null;
            try
            {

                url = await storage.Child("Profiles").Child(uid).GetDownloadUrlAsync().ConfigureAwait(false);
                Debug.WriteLine(url);
            }
            catch (FirebaseStorageException)
            {
                Debug.WriteLine("URL not found");
            }
            return url;
        }
        public static async Task<string> GetStoryURL(string uid, string key)
        {
            var url = await storage.Child("Stories").Child(uid).Child(key).GetDownloadUrlAsync().ConfigureAwait(false);
            if (url == null) Debug.WriteLine("URL is null");
            return url;
        }
        public static async Task<List<Story>> GetUserStories(string uid)
        {

            var stories = new List<Story>();
            try
            {
                var firebaseObjects = await firebase.Child("Stories").Child(uid).OnceAsync<StoryDataModel>().ConfigureAwait(false);
                if (firebaseObjects != null)
                {
                    foreach (var obj in firebaseObjects)
                    {
                        var location = obj.Object.location;

                        var s = new Story
                        {
                            location = location,
                            uid = uid,
                            url = await GetStoryURL(uid, obj.Key)
                        };
                        stories.Add(s);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error Getting user stories: " + e);
            }

            return stories;
        }
        
        public static async void AddLikeToPost(Post post)
        {
            try
            {
                await firebase.Child("Likes").Child(post.pid).Child(UID).PutAsync(true).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
        public static async void RemoveLikeFromPost(Post post)
        {
            try
            {
                await firebase.Child("Likes").Child(post.pid).Child(UID).DeleteAsync().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
}

        
        public static async void GoToParty(string pid)
        {
            try
            {
                await firebase.Child("Guests").Child(pid).Child(UID).PutAsync(true).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
        public static async void UndoGoToParty(string pid)
        {
            try
            {
                await firebase.Child("Guests").Child(pid).Child(UID).DeleteAsync().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        public static async void UpdateMusicPreferences(byte musicPreferences)
        {
            try
            {
                await CurrentUser.Child("music").PutAsync(musicPreferences).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Debug.Write(e.Message);
            }
        }
        public static async void UpdateBio(string bio)
        {
            try
            {
                await CurrentUser.Child("bio").PutAsync(bio).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Debug.Write(e.Message);
            }
        }
    }
}
