const functions = require('firebase-functions');
const admin = require('firebase-admin');
admin.initializeApp();

//  Entryを追加
exports.addEntry = functions
  .region('asia-northeast1')
  .https.onCall((data, context) =>
{
  if(context.auth === undefined || !context.auth.uid || context.auth.uid === 0){
    throw new functions.https.HttpsError('unauthenticated');
  }
  return admin.auth().getUser(context.auth.uid)
    .then(() =>
    {
      const entry = {
        name : data.name,
        score : data.score
      };
      return admin.firestore().collection('entries').add(entry)
    })
    .then((snapshot) =>
    {
      return 'OK';
    });
});

//  topEntriesを取得
exports.getTopEntries = functions
  .region('asia-northeast1')
  .https.onCall((data, context) =>
{
  if(context.auth === undefined || !context.auth.uid || context.auth.uid === 0){
    throw new functions.https.HttpsError('unauthenticated');
  }
  return admin.auth().getUser(context.auth.uid)
    .then(() =>
    {
      const count = data.count;
      return admin.firestore().collection('entries')
        .orderBy('score', 'desc')
        .limit(count)
        .get();
    })
    .then((qSnapshot) =>
    {
      return {
        entries : qSnapshot.docs.map(x => x.data())
      };
    });
});
