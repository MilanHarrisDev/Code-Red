const functions = require('firebase-functions');
const admin = require('firebase-admin');

admin.initializeApp();

var db = admin.firestore();

exports.getClientUsage = functions.https.onRequest((request, response) => {
    const clientId = request.query.clientId

    db.collection('clients').get()
    .then((snapshot) => {
      snapshot.forEach((doc) => {
        if(doc && doc.exists){
            if(doc.id == clientId)
            {
                var temp = JSON.parse(JSON.stringify(doc.data()));
                temp.id = clientId;
                response.status(200).send(temp);
            }
        }
      });
    })
    .catch((err) => {
      console.log('Error getting documents', err);
    });
})
