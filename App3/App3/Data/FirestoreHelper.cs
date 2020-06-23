using System;
using System.Collections.Generic;
using System.Text;
using Google.Cloud.Firestore;
using System.Threading.Tasks;
using App3.Models;
using Google.Cloud.Firestore.V1;
using System.Linq;

namespace App3.Data
{
    public static class FirestoreHelper
    {
        
        public static FirestoreDb db = FirestoreDb.Create("tribal-dispatch-276722");

        /*
         * User data model helper methods
         */

        public static async Task<bool> AddUser(User user)
        {
            return await db.Collection("Users").Document(user.uid).SetAsync(user) != null;
        }
        public static async Task<User> GetUser(string uid)
        {
            DocumentSnapshot snap = await db.Collection("Users").Document(uid).GetSnapshotAsync();
            if (snap.Exists)
            {
                User user = snap.ConvertTo<User>();
                user.uid = snap.Id;
                return user;
            }
            return null;
        }

        /*
         * Party data model helper methods
         */

        internal class PartyReferenceDocument
        {
            public string id;
            public bool pub;
        }
        public static async Task<bool> CreateParty(string uid, Party party, bool pub = true)
        {
            party.throwerid = uid;
            var p = await db.Collection("Parties").AddAsync(party);

            await db.Collection("Users").Document(uid).Collection("Parties").AddAsync(new PartyReferenceDocument { id = p.Id, pub = pub });
            return true;
        }
        public static List<Party> GetUserParties(string uid)
        {
            var list = new List<Party>();
            var doc = db.Collection("Users").Document(uid).Collection("Parties").StreamAsync().ForEachAsync(async item => {
                var pid = item.ConvertTo<PartyReferenceDocument>().id;
                var party = await GetParty(pid);
                if (party != null) list.Add(party);
            });
            return list;
        }
        public static async Task<Party> GetParty(string pid)
        {
            var doc = await db.Collection("Parties").Document(pid).GetSnapshotAsync();
            if (doc.Exists)
            {
                Party party = doc.ConvertTo<Party>();
                party.pid = pid;
                return party;
            }
            return null;
        }
        public static List<Party> GetInvites(string uid)
        {
            var list = new List<Party>();
            var doc = db.Collection("Users").Document(uid).Collection("Invites").StreamAsync().ForEachAsync(async item => {
                var pid = item.ConvertTo<PartyReferenceDocument>().id;
                var party = await GetParty(pid);
                if (party != null) list.Add(party);
            });
            return list;
        }
        public static async Task<List<Party>> GetPublicParties()
        {
            var list = new List<Party>();
            await db.Collection("Public").StreamAsync().ForEachAsync(item =>
            {
                var party = item.ConvertTo<Party>();
                party.pid = item.Id;
                if (party != null) list.Add(party);
            });
            return list;
        }
        /*public static async Task<bool> InviteUserToParty(string uid, string fid, string pid)
        {

        }*/
    }
}
