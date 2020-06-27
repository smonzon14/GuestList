
using App3.Models;
using Firebase.Database;
using Firebase.Database.Query;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using ZXing.OneD;
using Firebase.Storage;
using System.IO;

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
        public static List<User> FindUsersMatching(string query, System.Threading.CancellationToken ct)
        {
            if (query.Length == 0) return new List<User>();
            try
            {
                var task = firebase.Child("Users").OrderBy("name").StartAt(query).EndAt(query + "\uf8ff").LimitToFirst(15).OnceAsync<User>();

                var possibleUsers = (task.Result).Select(item => new User
                {
                    uid = item.Key,
                    name = item.Object.name,
                    bio = item.Object.bio,
                    status = item.Object.status,
                    email = item.Object.email
                }).ToList();
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
            Debug.WriteLine("Getting User: " + uid);
            try
            {
                User user = await firebase.Child("Users").Child(uid).OnceSingleAsync<User>().ConfigureAwait(false);
                return user;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }
        }
        public static async Task<bool> AddUser(User user)
        {
            Debug.WriteLine("Adding User...");
            try
            {
                var uid = user.uid;
                user.uid = null;
                await firebase.Child("Users").Child(uid).PutAsync(user).ConfigureAwait(false);
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

            var friendIdList = (await firebase.Child("Friendships").Child(userid).OnceAsync<bool>().ConfigureAwait(false)).Select(item => item.Key).ToList();

            List<User> friends = new List<User>();
            foreach (string u in friendIdList)
            {
                Debug.Write(u);
                var f = await GetUserFromUID(u);
                if (f != null)
                {
                    f.uid = u;
                    Debug.WriteLine(f);
                    friends.Add(f);
                }

            }
            return friends;
        }

        public static async Task<bool> AddFriend(string userid, string friendid)
        {
            if (userid.Equals(friendid)) return false;
            Debug.WriteLine("Adding Friend...");
            var fpath = firebase.Child("Friendships").Child(friendid);
            var upath = firebase.Child("Friendships").Child(userid).Child(friendid);

            try
            {
                await upath.PutAsync(true).ConfigureAwait(false);
                var existingRequest = await fpath.OrderByKey().EqualTo(userid).OnceAsync<bool>().ConfigureAwait(false);
                bool hasAccepted = false;
                if (existingRequest.Count < 1) await fpath.Child(userid).PutAsync(false).ConfigureAwait(false);
                else
                {
                    hasAccepted = existingRequest.First().Object;
                    Debug.WriteLine("Has Accepted: " + hasAccepted);
                }
                return hasAccepted;

            }
            catch (Exception e)
            {
                Debug.WriteLine("Error adding friend: " + e.Message);
            }
            return false;


        }
        public static async Task<bool> RemoveFriend(string userid, string friendid)
        {
            Debug.WriteLine("Removing Friend...");
            var fpath = firebase.Child("Friendships").Child(friendid);
            var upath = firebase.Child("Friendships").Child(userid).Child(friendid);

            try
            {
                await upath.DeleteAsync().ConfigureAwait(false);
                await fpath.Child(userid).DeleteAsync().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error removing friend: " + e.Message);
            }
            return true;
        }

        public static async Task<int> FriendStatus(string uid, string fid)
        {
            Debug.WriteLine("Retrieving Friend status of: " + fid);
            try
            {
                var fpath = firebase.Child("Friendships").Child(fid);
                var upath = firebase.Child("Friendships").Child(uid);

                var x = await fpath.OrderByKey().EqualTo(uid).OnceAsync<bool>().ConfigureAwait(false);
                var existingRequest = false;
                if (x.Count > 0) existingRequest = x.First().Object;

                var y = await upath.OrderByKey().EqualTo(fid).OnceAsync<bool>().ConfigureAwait(false);
                var existingFriend = false;
                if (y.Count > 0) existingFriend = y.First().Object;

                if (!existingRequest && !existingFriend) return 0; // No friend request either way
                if (!existingRequest && existingFriend) return 1; // Friend request sent
                if (existingRequest && !existingFriend) return 2; // Friend request recieved
                if (existingRequest && existingFriend) return 3; // Are friends
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            return -1;
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

        }

        public static async Task<bool> CreateParty(Party party, string userid, int exclusivity)
        {
            Debug.WriteLine("Creating Party...");
            try
            {
                var item = await firebase.Child("Parties").Child(userid).PostAsync(new PartyPost
                {
                    thrower = App.UserDatabase.GetUser().name,
                    address = party.address,
                    description = party.description,
                    name = party.name
                });
                if (item == null)
                {
                    Debug.WriteLine("Party Item is null");
                    return false;
                }
                return true;

            }
            catch (Exception e)
            {
                Debug.WriteLine("Could not Create party: " + e);
                return false;
            }
        }

        public static async Task<List<Party>> GetInvitedParties(string userid)
        {
            Debug.WriteLine("Getting Invites...");
            List<Party> parties = new List<Party>();
            var partiesList = firebase.Child("Invites").Child(userid).OnceAsync<string>().Result;
            if (partiesList == null) return parties;
            var partiesIdList = partiesList.Select(item => item.Object).ToList();

            Debug.WriteLine("OK");
            foreach (string id in partiesIdList)
            {
                var p = await GetParty(id, userid);
                if (p != null) parties.Add(p);
            }
            return parties;
        }

        /*public static async Task<List<Party>> GetPublicPartiesForRegion(double Longitude, double Latitude)
        {

        }*/
        public static async Task<Party> GetParty(string partyId, string throwerid)
        {
            Debug.WriteLine("Getting Party...");
            try
            {
                Party party = await firebase.Child("Parties").Child(throwerid).Child(partyId).OnceSingleAsync<Party>();
                return party;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Could Not find party with id (" + partyId + "): " + e);
                return null;
            }
        }

        public static async Task<List<Party>> GetPartiesThrownByUser(string uid)
        {
            Debug.WriteLine("Getting Parties...");
            var partiesList = new List<Party>();
            try
            {
                var firebaseObjects = (await firebase.Child("Parties").Child(uid).OnceAsync<Party>().ConfigureAwait(false));
                if (firebaseObjects != null)
                {
                    foreach (var obj in firebaseObjects)
                    {
                        var party = obj.Object;
                        party.pid = obj.Key;
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
        public static async Task<Comment> AddCommentToPost(Comment comment, string pid)
        {
            var c = await firebase.Child("Comments").Child(pid).PostAsync(comment).ConfigureAwait(false);
            var newComment = c.Object;
            newComment.cid = c.Key;
            return newComment;
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
                var firebaseObj = await firebase.Child("Stories").Child(story.uid).PostAsync(new StoryPostMetaData { location = "HEllo"}).ConfigureAwait(false);
                var key = firebaseObj?.Key;
                var storyRef = storage
                    .Child("Stories")
                    .Child(App.UserDatabase.GetUser().uid)
                    .Child(key);
                var url = await storyRef
                    .PutAsync(story.media);


                Debug.WriteLine("Story posted: " + url);
            } catch (Exception e)
            {
                Debug.WriteLine("Could not post story: " + e.Message);
            }
            
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
            await firebase.Child("Likes").Child(post.pid).Child(userid).PutAsync(true).ConfigureAwait(false);
        }
        public static async void RemoveLikeFromPost(Post post, string userid) 
        {
            await firebase.Child("Likes").Child(post.pid).Child(userid).DeleteAsync().ConfigureAwait(false);
        }
    }
}
