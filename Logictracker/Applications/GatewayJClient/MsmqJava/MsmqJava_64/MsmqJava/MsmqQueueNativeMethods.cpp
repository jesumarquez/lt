// MsmqQueueNativeMethods.cpp 
//
// Native methods for MSMQ APIs, to be exposed into the Java Queue class via JNI.
//

#include <stdio.h>
#include <WTypes.h>   // reqd for WinBase.h
#include <WinBase.h>  // for CriticalSection
#include <MqOai.h>
#include <mq.h>

#include "JniMsmq.h"
#include "MsmqQueue.hpp"


// NB: 
//
// The C lib for MSMQ requires that queues are opened with either RECEIVE
// or SEND access, but not both.  Because we want the Java Queue class to
// support both reading and writing, we therefore maintain two queue
// handles for each queue: one for reading and one for writing.  Therefore
// to handle 10 queues, we need to maintain slots for 20 queue handles.
// 
// For queue N, the receiving handle is stored in qhandles[N*2],
// and the sending handle is qhandles[N*2+1].
//

#define MAX_QUEUES 10
static	MsmqQueue *qhandles[MAX_QUEUES * 2];
static  int isInited = -1;

// Global variable
static CRITICAL_SECTION CriticalSection; 


void InitQueueHandles() 
{
    int i;
    for (i=0;i < MAX_QUEUES; i++) {
	qhandles[i*2]= NULL;  // receive handle
	qhandles[i*2+1]= NULL; // send handle
    }
}


int GetNextFreeHandleSlot() 
{
    int i, selected=-1;
    for (i=0; (i < MAX_QUEUES) && (selected==-1); i++) {
	if (qhandles[i*2] == NULL) {
	    selected= i;
	}
    }
    return selected;
}



void FreeSlot(int slot) {
    EnterCriticalSection(&CriticalSection); 
    qhandles[slot*2] = NULL;
    qhandles[slot*2+1] = NULL;
    LeaveCriticalSection(&CriticalSection); 
}


int StoreHandles(MsmqQueue * receiver, MsmqQueue * sender ) {
    EnterCriticalSection(&CriticalSection); 

    int slot= GetNextFreeHandleSlot();
    if (slot>=MAX_QUEUES) slot=-1;
    if (slot!=-1) {
	qhandles[slot*2]= receiver; // receive handle
	qhandles[slot*2+1]= sender; // send handle
    }

    LeaveCriticalSection(&CriticalSection); 
    return slot; 
}



int GetQueueSlot(JNIEnv *jniEnv, jobject object, HRESULT *p_hr) {
    jclass cls = jniEnv->GetObjectClass(object);
    jfieldID fieldId;
    jint queueSlot= -1;
    fieldId = jniEnv->GetFieldID(cls, "_queueSlot", "I");
    if (fieldId == 0) {
	*p_hr = -5;
	return NULL;
    }

    queueSlot = jniEnv->GetIntField(object, fieldId);
    if (queueSlot >= MAX_QUEUES) {
	*p_hr = -2;
	return NULL;
    }

    return (int) queueSlot;
}


MsmqQueue*  GetQueue(JNIEnv *jniEnv, jobject object, int * pSlot, HRESULT *p_hr, int flavor) {
    if ((flavor!=0)&&(flavor!=1)) {
	*p_hr= -7;
	return NULL; 
    }
    int slot= GetQueueSlot(jniEnv, object, p_hr);
    if (pSlot!=NULL) *pSlot= slot;

    EnterCriticalSection(&CriticalSection); 
    MsmqQueue *p= qhandles[slot*2+flavor];
    LeaveCriticalSection(&CriticalSection); 

    return p;
}



MsmqQueue*  GetSenderQueue(JNIEnv *jniEnv, jobject object, int * pSlot, HRESULT *p_hr) {
    return GetQueue(jniEnv, object, pSlot, p_hr, 1);
}

MsmqQueue*  GetReceiverQueue(JNIEnv *jniEnv, jobject object, int * pSlot, HRESULT *p_hr) {
    return GetQueue(jniEnv, object, pSlot, p_hr, 0);
}




void SetJavaString (JNIEnv * jniEnv, jobject object, char * fieldName, const char * valueToSet) 
{
    jclass cls = jniEnv->GetObjectClass(object);

    jstring value = jniEnv->NewStringUTF(valueToSet);
    jfieldID fieldId;
    fieldId = jniEnv->GetFieldID(cls, fieldName, "Ljava/lang/String;");
    if (fieldId !=0)
	jniEnv->SetObjectField(object, fieldId, value);
}




// static // 
JNIEXPORT jint JNICALL Java_ionic_Msmq_Queue_nativeInit
 	(JNIEnv *jniEnv, jclass clazz)

{
    // to be called once, by static initializer in Java class
    InitializeCriticalSection(&CriticalSection) ;
    InitQueueHandles(); 
    return 0;
}





// static // 
JNIEXPORT jint JNICALL Java_ionic_Msmq_Queue_nativeCreateQueue
 	(JNIEnv *jniEnv, jclass clazz, jstring queuePath, jstring queueLabel, jint isTransactional)

{
    HRESULT hr = 0;
    try {
	MsmqQueue   *q = new MsmqQueue();

	const char * szQueuePath = jniEnv->GetStringUTFChars(queuePath, 0);
	const char * szQueueLabel = jniEnv->GetStringUTFChars(queueLabel, 0);
	WCHAR wszFormatName[256]; // MQ_MAX_Q_NAME_LEN ?
	DWORD dwFormatNameBufferLength = 256;

	hr = q->createQueue((char *) szQueuePath, 
					(char *) szQueueLabel, 
					(LPWSTR) wszFormatName,			
					&dwFormatNameBufferLength,
					isTransactional);

	jniEnv->ReleaseStringUTFChars(queuePath, szQueuePath);
	jniEnv->ReleaseStringUTFChars(queueLabel, szQueueLabel);
    }
    catch(...) {
	printf("createQueue caught an error..\n");
	jniEnv->ExceptionDescribe();
	jniEnv->ExceptionClear();
	hr = -99;
    }

    fflush(stdout);
    return (jint) hr;
}


// static // 
JNIEXPORT jint JNICALL Java_ionic_Msmq_Queue_nativeDeleteQueue
 	(JNIEnv *jniEnv, jclass clazz, jstring queuePath)

{
    HRESULT hr = 0;
    try {
	MsmqQueue   *q = new MsmqQueue();

	const char * szQueuePath = jniEnv->GetStringUTFChars(queuePath, 0);

	hr = q->deleteQueue((char *) szQueuePath) ; 

	jniEnv->ReleaseStringUTFChars(queuePath, szQueuePath);
    }
    catch(...) {
	printf("deleteQueue caught an error..\n");
	jniEnv->ExceptionDescribe();
	jniEnv->ExceptionClear();
	hr = -99;
    }

    fflush(stdout);
    return (jint) hr;
}



/// not a JNI call ///
jint OpenQueueWithAccess
	(JNIEnv *jniEnv, jobject object, jstring queuePath, int access)
{
    jclass cls = jniEnv->GetObjectClass(object);
    jfieldID fieldId;
    HRESULT hr;

    try {
	MsmqQueue * sender= NULL;
	MsmqQueue * receiver= NULL;

	const char *szQueuePath = jniEnv->GetStringUTFChars(queuePath, 0);
	printf("OpenQueueWithAccess (%s)\n", szQueuePath ); 

	if (access & MQ_RECEIVE_ACCESS) {
	    receiver = new MsmqQueue();
	    // dinoch - Wed, 11 May 2005  13:48
	    // MQ_ADMIN_ACCESS == use the local, outgoing queue for remote queues. ?? 
	    hr = receiver->openQueue((char *) szQueuePath, MQ_RECEIVE_ACCESS ); // | MQ_ADMIN_ACCESS 
	    if (hr != 0) {
		delete receiver;
		return hr;
	    }
	}

	if (access & MQ_SEND_ACCESS) {
	    sender = new MsmqQueue();
	    // dinoch - Wed, 11 May 2005  13:48
	    // MQ_ADMIN_ACCESS == use (local) outgoing queue for remote queues
	    hr = sender->openQueue((char *) szQueuePath, MQ_SEND_ACCESS ); // | MQ_ADMIN_ACCESS 
	    if (hr != 0) {
		delete sender;
		if (receiver!=NULL) delete receiver;
		return hr;
	    }
	}

	jniEnv->ReleaseStringUTFChars(queuePath, szQueuePath);

	int slot= StoreHandles(receiver, sender);
	if (slot == -1) return -2;

	fieldId = jniEnv->GetFieldID(cls, "_queueSlot", "I");
	if (fieldId == 0) return -3;
	jniEnv->SetIntField(object, fieldId, (jint)slot);
    }
    catch(...) {
	printf("openQueue : Exception. \n");
    	jniEnv->ExceptionDescribe();
    	jniEnv->ExceptionClear();
	hr = -99;
    }

    fflush(stdout);

    return (jint) hr;
}


// static // 
JNIEXPORT jint JNICALL Java_ionic_Msmq_Queue_nativeOpenQueue
	(JNIEnv *jniEnv, jobject object, jstring queuePath)
{
    return OpenQueueWithAccess(jniEnv, object, queuePath, MQ_SEND_ACCESS | MQ_RECEIVE_ACCESS);
}


// static // 
JNIEXPORT jint JNICALL Java_ionic_Msmq_Queue_nativeOpenQueueForSend
	(JNIEnv *jniEnv, jobject object, jstring queuePath)
{

    return OpenQueueWithAccess(jniEnv, object, queuePath, MQ_SEND_ACCESS);
}

// static // 
JNIEXPORT jint JNICALL Java_ionic_Msmq_Queue_nativeOpenQueueForReceive
	(JNIEnv *jniEnv, jobject object, jstring queuePath)
{

    return OpenQueueWithAccess(jniEnv, object, queuePath, MQ_RECEIVE_ACCESS);
}








JNIEXPORT jint JNICALL Java_ionic_Msmq_Queue_nativeReceive
 	(JNIEnv *jniEnv, jobject object, jint MaxMessageLength, jint timeout, jint ReadOrPeek)

{
    HRESULT  hr = 0;

    try {
	MsmqQueue *q = GetReceiverQueue(jniEnv, object, NULL, &hr);
	if (hr!=0) return (jint) hr;

	char *szMessage= (char *) malloc(MaxMessageLength); 
	char szCorrelationID[PROPID_M_CORRELATIONID_SIZE];
	char szLabel[MQ_MAX_MSG_LABEL_LEN];

	hr = q->read((char *) szMessage, 
				 (int)MaxMessageLength, 
				 (char *) szCorrelationID, 
				 (char *) szLabel, 
				 timeout, 
				 ReadOrPeek);

	if (hr==0) {
	    SetJavaString(jniEnv, object, "_lastMessageRetrieved_MessageString", szMessage); 
	    SetJavaString(jniEnv, object, "_lastMessageRetrieved_MessageLabel", szLabel); 
	    SetJavaString(jniEnv, object, "_lastMessageRetrieved_CorrelationId", szCorrelationID); 
	}
	free(szMessage);
	if (hr!=0)  return hr;
    }
    catch(...) {
	printf("Read() : Exception\n");
	jniEnv->ExceptionDescribe();
	jniEnv->ExceptionClear();
	hr = -99;
    }

    fflush(stdout);
    return (jint) hr;
}







JNIEXPORT jint JNICALL Java_ionic_Msmq_Queue_nativeSend
 	(JNIEnv *jniEnv, 
	 jobject object,
	 jstring message,
	 jint jMessageLength,
	 jstring label,
	 jstring correlationID, 
	 jint transactionFlag)

{
    HRESULT hr = 0;
    try {
	MsmqQueue   *q = GetSenderQueue(jniEnv, object, NULL, &hr);
	if (hr!=0) return (jint) hr;

	int messageLength = jMessageLength;

	const char *szMessage= jniEnv->GetStringUTFChars(message, 0);
	const char *szCorrelationID = jniEnv->GetStringUTFChars(correlationID, 0);
	const char *szLabel = jniEnv->GetStringUTFChars(label, 0);

	hr = q->sendMessage(
			    (char *) szMessage, 
			    messageLength, 
			    (char *) szLabel, 
			    (char *) szCorrelationID, 
			    transactionFlag);

	jniEnv->ReleaseStringUTFChars(message, szMessage);
	jniEnv->ReleaseStringUTFChars(correlationID, szCorrelationID);
	jniEnv->ReleaseStringUTFChars(label, szLabel);
    }
    catch(...) {
	jniEnv->ExceptionDescribe();
	jniEnv->ExceptionClear();
	hr = -99;
    }

    fflush(stdout);
    return (jint) hr;
}






JNIEXPORT jint JNICALL Java_ionic_Msmq_Queue_nativeClose
 		(JNIEnv *jniEnv, jobject object)

{
    HRESULT hr_r = 0;
    HRESULT hr_s = 0;
    HRESULT hr = 0;
    int slot; 
    try {
	MsmqQueue *r,*s;
	r = GetReceiverQueue(jniEnv, object, &slot, &hr_r);
	s = GetSenderQueue(jniEnv, object, &slot, &hr_s);
	if ((hr_r==0) && (r!=NULL)){
	    hr_r = r->closeQueue();
	    delete r;
	    if (hr_r!=0) printf("Zowie, can't close receiver. (hr=0x%08x)\n", hr_r);
	}

	if ( (hr_s==0) && (s!=NULL) ) {
	    hr_s = s->closeQueue();
	    delete s;
	    if (hr_s!=0) printf("Zowie, can't close sender. (hr=0x%08x)\n", hr_s);
	}

	FreeSlot(slot);

	// we return at most one of the HRESULTs 
	if (hr_r!=0) hr= hr_r;
	else if (hr_s!=0) hr= hr_s;

    }
    catch(...) {
	jniEnv->ExceptionDescribe();
	jniEnv->ExceptionClear();
	hr = -99;
    }

    fflush(stdout);
    return (jint) hr;
}

