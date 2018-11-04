import firebase_admin
import random
import time
from firebase_admin import credentials
from firebase_admin import firestore
from apscheduler.schedulers.background import BackgroundScheduler

# Use a service account
cred = credentials.Certificate('codered-22add34f7d4b.json')
firebase_admin.initialize_app(cred)

db = firestore.client()

def SetRandomUpDown():
    doc_ref = db.collection(u'clients').document(u'google_home')
    doc_ref.set({
        u'bytesIn': random.randint(800, 2000),
        u'bytesOut': random.randint(800, 2000)
    })

scheduler = BackgroundScheduler()
scheduler.add_job(SetRandomUpDown, 'interval', seconds = 1)
scheduler.start()

if __name__ == '__main__':
    try:
        while True:
            time.sleep(2)
    except (KeyboardInterrupt, SystemExit):
            scheduler.shutdown()



