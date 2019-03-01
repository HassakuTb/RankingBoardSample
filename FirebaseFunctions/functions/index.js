const functions = require('firebase-functions');
const admin = require('firebase-admin');
admin.initializeApp();

//  Entryを追加
exports.addEntry = functions
  .region('asia-northeast1')
  .https.onCall((data, context) =>
{
  if(context.auth === undefined){
    throw new functions.https.HttpsError('unauthenticated');
  }

  const entry = {
    name : data.name,
    score : data.score
  };
  return admin.firestore().collection('entries')
    .add(entry)
    .then((snapshot) =>
  {
    return 'OK';
  });
});

//  TopEntryを取得
exports.getTopEntries = functions
  .region('asia-northeast1')
  .https.onCall((data, context) =>
{
  if(context.auth === undefined){
    throw new functions.https.HttpsError('unauthenticated');
  }

  const count = data.count;

  return admin.firestore().collection('entries')
    .orderBy('score', 'desc')
    .limit(count)
    .get()
    .then((qSnapshot) =>
  {
    return {
      entries : qSnapshot.docs.map(x => x.data())
    };
  });
});
