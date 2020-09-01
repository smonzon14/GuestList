const functions = require('firebase-functions');
const admin = require('firebase-admin');
admin.initializeApp();
// Create and Deploy Your First Cloud Functions
// https://firebase.google.com/docs/functions/write-firebase-functions
// exports.removeFriend = functions.database.ref('/Friendships/{MyUid}/{FriendUid}').onDelete(async (change, context) =>{
//       const MyUid = context.params.MyUid;
//       const FriendUid = context.params.FriendUid; 
//       admin.database().ref(`/Friendships/${FriendUid}/${MyUid}`).remove((error)=>{
//         console.log("Error removing friend. " + error.message);
//       });
// });
exports.updateFriendStatus = functions.database.ref('/Friendships/{MyUid}/{FriendUid}')
    .onWrite(async (change, context) => {
      const MyUid = context.params.MyUid;
      const FriendUid = context.params.FriendUid; 
      
      if(!change.after.exists()){
        
        return admin.database().ref(`/Friendships/${FriendUid}/${MyUid}`).remove((error)=>{
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
      const getPreviousFriendRequest = admin.database().ref(`/Friendships/${FriendUid}/${MyUid}`).once("value");

      let results = await Promise.all([getPreviousFriendRequest]);
      const returningFriendRequest = results[0].val() === 1;
      // Get the list of device notification tokens.
      const getDeviceTokensPromise = admin.database().ref(`/Users/${returningFriendRequest ? FriendUid : MyUid}/notificationTokens`).once('value');

      // Get the profile.
      const getRespondingProfilePromise = admin.database().ref(`/Users/${MyUid}/name`).once('value');
      
      results = await Promise.all([getDeviceTokensPromise, getRespondingProfilePromise]);
      
      const tokensSnapshot = results[0];
      const name = results[1].val();
      let payload;

      if(returningFriendRequest){
        admin.database().ref(`/Friendships/${FriendUid}/${MyUid}`).set(3);
        admin.database().ref(`/Friendships/${MyUid}/${FriendUid}`).set(3);
        payload = {
          notification: {
            title: `${name}`,
            body: 'is now your friend!'
          }
        };
      }else{
        admin.database().ref(`/Friendships/${FriendUid}/${MyUid}`).set(2);
        payload = {
          notification: {
            title: `${name}`,
            body: `has requested to be your friend!`
          }
        };
      }

      

      // Check if there are any device tokens.
      if (!tokensSnapshot.hasChildren()) {
        return console.log('There are no notification tokens to send to.');
      }
      console.log('There are', tokensSnapshot.numChildren(), 'tokens to send notifications to.');
      console.log('Fetched follower profile', name);

      

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
    });