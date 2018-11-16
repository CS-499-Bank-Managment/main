import sqlite3
import datetime
import time
import sched

AcctIndex = {
	"owner_id": 0,
	"acct_id": 1,
	"balance": 2,
	"type": 3,
	"name": 4,
	"interest": 5,
	"created": 6,
	"edited": 7
}

def eventloop(sc):
	conn = sqlite3.connect("../Accounts.sqlite")
	
	cursor = conn.cursor()
	
	cursor.execute("SELECT * from customer_acct")
	accts = cursor.fetchall()
	for row in accts:
		id = row[AcctIndex["acct_id"]]
		newbalance = round(row[AcctIndex["balance"]] + (row[AcctIndex["balance"]] * row[AcctIndex["interest"]]), 2)
		cursor.execute(f"UPDATE customer_acct SET balance = {newbalance}, "
		               f"edited = '{datetime.datetime.now().replace(microsecond=0)}'"
		               f" WHERE acct_id = {id}")
	conn.commit()
	conn.close()
	s.enter(60, 1, eventloop, (sc,))
	
s = sched.scheduler(time.time, time.sleep)
s.enter(60, 1, eventloop, (s,))
s.run()