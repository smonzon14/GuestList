
using App3.Models;
using Firebase.Database;
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

        public static FirebaseClient firebase = new FirebaseClient("https://tribal-dispatch-276722.firebaseio.com/");
        public static FirebaseStorage storage = new FirebaseStorage("tribal-dispatch-276722.appspot.com");
        
        /*
         * User
         */
        internal class UserBase
        {
            public string uid { get; set; }
            public string bio { get; set; }
            public int status { get; set; }
            public string name { get; set; }
            public string email { get; set; }
        }
        public static async Task<List<User>> FindUsersMatching(string query)
        {
            if (query.Length == 0) return new List<User>();
            try
            {
                var task = firebase.Child("Users").OrderBy("name").StartAt(query).EndAt(query + "\uf8ff").LimitToFirst(15).OnceAsync<UserBase>().ConfigureAwait(false);

                var possibleUsers = (await task).Select(item => new User
                {
                    uid = item.Key,
                    name = item.Object.name,
                    bio = item.Object.bio,
                    status = item.Object.status,
                    email = item.Object.email
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
                UserBase user = await firebase.Child("Users").Child(uid).OnceSingleAsync<UserBase>().ConfigureAwait(false);
                Debug.Write("Got User: ");
                //user.printUser();
                var u = new User { name = user.name, uid = uid, bio = user.bio, status = user.status, email = user.email };
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
                
                var data = new UserBase { bio = user.bio, name = user.name, email = user.email, status = user.status };
                await firebase.Child("Users").Child(user.uid).PutAsync(data).ConfigureAwait(false);
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error:{e}");
                return false;
            }
        }

        public static async Task<bool> DeleteUser(string uid)
        {
            try
            {
                await firebase.Child("Users").Child(uid).DeleteAsync().ConfigureAwait(false);

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

        public static async Task<List<User>> GetFriendsList(string userid)
        {
            Debug.WriteLine("Getting Friends List...");

            var friendIdList = (await firebase.Child("Friendships").Child(userid).OnceAsync<int>().ConfigureAwait(false));

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
        public static async Task<bool> AddFriend(string userid, string friendid)
        {
            if (userid.Equals(friendid)) return false;
            Debug.WriteLine("Adding Friend...");

            var fpath = firebase.Child("Friendships").Child(friendid).Child(userid);
            var upath = firebase.Child("Friendships").Child(userid).Child(friendid);

            
            int friendStatus;
            try
            {
                friendStatus = await fpath.OnceSingleAsync<int>().ConfigureAwait(false);
            }
            catch
            {
                friendStatus = 0;
            }
            try
            {
                switch (friendStatus)
                {
                    case 0:
                        await upath.PutAsync(1).ConfigureAwait(false);
                        await fpath.PutAsync(2).ConfigureAwait(false);
                        break;
                    case 1:
                        await upath.PutAsync(3).ConfigureAwait(false);
                        await fpath.PutAsync(3).ConfigureAwait(false);
                        break;
                    case 2:
                        await upath.PutAsync(1).ConfigureAwait(false);
                        break;
                    case 3:
                        await upath.PutAsync(3).ConfigureAwait(false);
                        break;
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        public static async Task<bool> RemoveFriend(string userid, string friendid)
        {
            Debug.WriteLine("Removing Friend...");
            var fpath = firebase.Child("Friendships").Child(friendid).Child(userid);
            var upath = firebase.Child("Friendships").Child(userid).Child(friendid);

            try
            {
                await upath.DeleteAsync().ConfigureAwait(false);
                await fpath.DeleteAsync().ConfigureAwait(false);
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

        internal class PartyPost
        {
            public string name { get; set; }
            public string description { get; set; }
            public int numPeopleGoing
            {
                get; set;
            }
            public string thrower { get; set; }
            public string address { get; set; }
            public bool img { get; set; }
            public DateTime time { get; set; }
        }

        public static async Task<bool> CreateParty(Party party, string userid, Plugin.Media.Abstractions.MediaFile mediaFile, int exclusivity)
        {
            Debug.WriteLine("Creating Party...");
            try
            {
                var item = await firebase.Child("Parties").Child(userid).PostAsync(new PartyPost
                {
                    thrower = App.UserDatabase.GetUser().name,
                    
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
            foreach (User friend in friends)
            {
                var partiesList = await firebase.Child("Parties").Child(friend.uid).OnceAsync<Party>().ConfigureAwait(false);
                if (partiesList != null)
                {
                    foreach(var obj in partiesList)
                    {
                        Party party = obj.Object;
                        party.Thrower = friend;
                        party.pid = obj.Key;
                        parties.Add(party);
                    }
                }
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
                Party party = await firebase.Child("Parties").Child(thrower.uid).Child(partyId).OnceSingleAsync<Party>();
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
        public static async void CreatePost(Post post, string userid)
        {
            try
            {

                var p = await firebase.Child("Posts").Child(userid).PostAsync(post).ConfigureAwait(false);
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
        internal class StoryPostMetaData
        {
            public string location { get; set; }
        }
        public static async void PostStory(Story story)
        {
            try
            {
                var firebaseObj = await firebase.Child("Stories").Child(story.uid).PostAsync(new StoryPostMetaData { location = "HEllo" }).ConfigureAwait(false);
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
        public static async Task<string> SetProfileImage(string uid, Plugin.Media.Abstractions.MediaFile mediaFile)
        {
            string url = null;
            try
            {
                var storyRef = storage.Child("Profiles").Child(uid);

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
                var firebaseObjects = await firebase.Child("Stories").Child(uid).OnceAsync<StoryPostMetaData>().ConfigureAwait(false);
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
        
        public static async void AddLikeToPost(Post post, string userid)
        {
            try
            {
                await firebase.Child("Likes").Child(post.pid).Child(userid).PutAsync(true).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
        public static async void RemoveLikeFromPost(Post post, string userid)
        {
            try
            {
                await firebase.Child("Likes").Child(post.pid).Child(userid).DeleteAsync().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
}

        
        public static async void GoToParty(string pid, string uid)
        {
            try
            {
                await firebase.Child("Guests").Child(pid).Child(uid).PutAsync(true).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
        public static async void UndoGoToParty(string pid, string uid)
        {
            try
            {
                await firebase.Child("Guests").Child(pid).Child(uid).DeleteAsync().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
    }
}
