using App3.Models;
using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace App3.Data
{
    public interface IZooPassDatabase
    {
        Task<bool> CreateParty(Party party, string userid, List<string> invites);
        Task<bool> InviteToParty(string pid, List<string> uids);
        Task<List<Party>> GetInvitedParties(string userid);
        Task<List<User>> GetFriendsList(string userid);
        Task<bool> AddFriend(string userid, string friendid);
        Task<bool> AcceptFriendRequest(string userid, string friendid);
        Task<Party> GetParty(string partyId);
        Task<List<Party>> GetPartiesThrownByUser(string uid);
        Task<User> GetUserByUID(string uid);
        Task<bool> AddUser(User user);
        Task<bool> DeleteUser(string email);
    }
}
