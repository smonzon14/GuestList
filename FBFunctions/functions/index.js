const functions = require('firebase-functions');
const admin = require('firebase-admin');
admin.initializeApp();

var db = admin.database();

// Create and Deploy Your First Cloud Functions
// https://firebase.google.com/docs/functions/write-firebase-functions
// exports.removeFriend = functions.database.ref('/Friendships/{MyUid}/{FriendUid}').onDelete(async (change, context) =>{
//       const MyUid = context.params.MyUid;
//       const FriendUid = context.params.FriendUid; 
//       admin.database().ref(`/Friendships/${FriendUid}/${MyUid}`).remove((error)=>{
//         console.log("Error removing friend. " + error.message);
//       });
// });
async function sendNotifications(tokensSnapshot, payload){
  if(!tokensSnapshot.hasChildren()) {return;}
  // The array containing all the user's tokens.
  // Listing all tokens as an array.
  let tokens = Object.keys(tokensSnapshot.val());
  // Send notifications to all tokens.
  const response = await admin.messaging().sendToDevice(tokens, payload);
  // For each message check if there was an error.
  const tokensToRemove = [];
  response.results.forEach((result, index) => {
    const error = result.error;
    if (error) {
      console.error('Failure sending notification to', tokens[index], error);
      // Cleanup the tokens who are not registered anymore.
      if (error.code === 'messaging/invalid-registration-token' ||
          error.code === 'messaging/registration-token-not-registered') {
        tokensToRemove.push(tokensSnapshot.ref.child(tokens[index]).remove());
      }
    }
  });
  return Promise.all(tokensToRemove);
}
exports.notifyHostOfGuest = functions.database.ref('/Guests/{HostUid}/{Pid}/{GuestUid}')
    .onWrite(async (change, context) => {
      const Pid = context.params.Pid;
      const HostUid = context.params.HostUid;
      const GuestUid = context.params.GuestUid;

      // Get guest count
      const acceptedInvite = change.after.exists();
      db.ref(`/Parties/${HostUid}/${Pid}/guestCount`).transaction((current) => (current || 0) + (acceptedInvite ? 1 : -1));

      if(!acceptedInvite){ return; }

      // Get the list of device notification tokens.
      const getDeviceTokensPromise = db.ref(`/Users/${HostUid}/notificationTokens`).once('value');
      
      // Get the guest name
      const getRespondingProfilePromise = db.ref(`/Users/${GuestUid}/name`).once('value');

      results = await Promise.all([getDeviceTokensPromise, getRespondingProfilePromise]);
      
      const tokensSnapshot = results[0];
      const name = results[1].val();

      const payload = {
        notification: {
          title: `${name}`,
          body: 'is going to your event!'
        }
      };

      return sendNotifications(tokensSnapshot, payload);
      
    });
exports.updateFriendStatus = functions.database.ref('/Friendships/{MyUid}/{FriendUid}')
    .onWrite(async (change, context) => {
      const MyUid = context.params.MyUid;
      const FriendUid = context.params.FriendUid; 
      
      if(!change.after.exists()){
        
        return db.ref(`/Friendships/${FriendUid}/${MyUid}`).remove((error)=>{
          console.log("Error removing friend. " + error.message);
        });
      }
      if(change.after.val() !== 1) {return;}
      // If un-follow we exit the function.
      //if (change.after.val() == 2) {
      //  return console.log('User ', MyUid, 'un-followed user', FriendUid);
      //}
      //let friendStatus = change.after.val(); 
      //if(friendStatus === 1) {
      //  return;
      //}
      console.log('Friend request from:', MyUid, ', to user:', FriendUid);

      
      // Get any previous friend request from prospective friend
      const getPreviousFriendRequest = db.ref(`/Friendships/${FriendUid}/${MyUid}`).once("value");

      let results = await Promise.resolve(getPreviousFriendRequest);
      const returningFriendRequest = results.val() === 1;
      // Get the list of device notification tokens.
      const getDeviceTokensPromise = db.ref(`/Users/${returningFriendRequest ? FriendUid : MyUid}/notificationTokens`).once('value');

      // Get the profile.
      const getRespondingProfilePromise = db.ref(`/Users/${MyUid}/name`).once('value');
      
      results = await Promise.all([getDeviceTokensPromise, getRespondingProfilePromise]);
      
      const tokensSnapshot = results[0];
      const name = results[1].val();
      let payload;

      if(returningFriendRequest){
        db.ref(`/Friendships/${FriendUid}/${MyUid}`).set(3);
        db.ref(`/Friendships/${MyUid}/${FriendUid}`).set(3);
        payload = {
          notification: {
            title: `${name}`,
            body: 'is now your friend!'
          }
        };
      }else{
        db.ref(`/Friendships/${FriendUid}/${MyUid}`).set(2);
        payload = {
          notification: {
            title: `${name}`,
            body: `has requested to be your friend!`
          }
        };
      }
      return sendNotifications(tokensSnapshot, payload);
      
    });