// MsmqQueue.cpp 
//
// See the MQ Functions Ref: 
// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/msmq/msmq_ref_functions_4o37.asp

#include <stdio.h>
#include <MqOai.h>
#include <mq.h>

#include "MsmqQueue.hpp"


HRESULT MsmqQueue::createQueue( char *szQueuePath, 
				  char *szQueueLabel, 
				  LPWSTR wszFormatName, 
				  DWORD * p_dwFormatNameBufferLength,
				  int isTransactional )
{
    HRESULT hr = MQ_OK;
    int len;

    // example: http://msdn.microsoft.com/library/default.asp?url=/library/en-us/msmq/msmq_using_createqueue_0f51.asp
    const int NUMBEROFPROPERTIES = 5;


    MQQUEUEPROPS   QueueProps;
    MQPROPVARIANT  aQueuePropVar[NUMBEROFPROPERTIES];
    QUEUEPROPID    aQueuePropId[NUMBEROFPROPERTIES];
    HRESULT        aQueueStatus[NUMBEROFPROPERTIES];
    DWORD          i = 0;
  
    if (szQueuePath == NULL) 
	    return MQ_ERROR_INVALID_PARAMETER;

    WCHAR wszPathName[MQ_MAX_Q_NAME_LEN];
    len= strlen(szQueuePath);
    if (MultiByteToWideChar(
			    (UINT) CP_ACP,
			    (DWORD) 0,
			    (LPCSTR) szQueuePath,
			    len,
			    (LPWSTR) wszPathName,
			    (int) sizeof( wszPathName ) ) == 0)
	{
	    return MQ_ERROR_INVALID_PARAMETER;
	}

    if (len < sizeof( wszPathName ) )
	wszPathName[len]= 0; // need this to terminate

    WCHAR wszLabel[MQ_MAX_Q_LABEL_LEN];
    len= strlen(szQueueLabel);
    if (MultiByteToWideChar(
			    (UINT) CP_ACP,
			    (DWORD) 0,
			    (LPCSTR) szQueueLabel,
			    len,
			    (LPWSTR) wszLabel,
			    (int) sizeof( wszLabel ) ) == 0)
	{
	    return MQ_ERROR_INVALID_PARAMETER;
	}
    if (len < sizeof( wszLabel ) )
	wszLabel[len]= 0; // need this to terminate


    printf("attempting to create queue with name= '%S', label='%S'\n", wszPathName, wszLabel);

    // Set the PROPID_Q_PATHNAME property with the path name provided.
    aQueuePropId[i] = PROPID_Q_PATHNAME;
    aQueuePropVar[i].vt = VT_LPWSTR;
    aQueuePropVar[i].pwszVal = wszPathName; // wszActualName
    i++;

    // Set optional queue properties. PROPID_Q_TRANSACTIONAL
    // must be set to make the queue transactional.
    aQueuePropId[i] = PROPID_Q_TRANSACTION;
    aQueuePropVar[i].vt = VT_UI1;
    aQueuePropVar[i].bVal = (unsigned char) isTransactional;
    i++;

    aQueuePropId[i] = PROPID_Q_LABEL;
    aQueuePropVar[i].vt = VT_LPWSTR;
    aQueuePropVar[i].pwszVal = wszLabel;
    i++;

    // Initialize the MQQUEUEPROPS structure 
    QueueProps.cProp = i;                  //Number of properties
    QueueProps.aPropID = aQueuePropId;     //IDs of the queue properties
    QueueProps.aPropVar = aQueuePropVar;   //Values of the queue properties
    QueueProps.aStatus = aQueueStatus;     //Pointer to return status


    // http://msdn.microsoft.com/library/en-us/msmq/msmq_ref_functions_8dut.asp
    hr = MQCreateQueue(NULL,                // Security descriptor
		       &QueueProps,         // Address of queue property structure
		       wszFormatName,       // Pointer to format name buffer
		       p_dwFormatNameBufferLength);  // Pointer to receive the queue's format name length
    return hr;

};



HRESULT MsmqQueue::deleteQueue( char *szQueuePath )
{
    HRESULT hr = MQ_OK;
    int len;

    // example: http://msdn.microsoft.com/library/default.asp?url=/library/en-us/msmq/msmq_using_createqueue_0f51.asp
    if (szQueuePath == NULL) 
	    return MQ_ERROR_INVALID_PARAMETER;

    // dinoch - Mon, 18 Apr 2005  16:34
    // removed the prefix.  The caller is now responsible for providing it. 
    //CHAR szDestFormatName[MQ_MAX_Q_NAME_LEN] = "DIRECT=OS:";
    //strcat(szDestFormatName, szQueuePath);
    CHAR szDestFormatName[MQ_MAX_Q_NAME_LEN];
    strncpy(szDestFormatName, szQueuePath, MQ_MAX_Q_NAME_LEN);

    WCHAR wszPathName[MQ_MAX_Q_NAME_LEN];
    len= strlen(szDestFormatName);
    if (MultiByteToWideChar(
			    (UINT) CP_ACP,
			    (DWORD) 0,
			    (LPCSTR) szDestFormatName,
			    len,
			    (LPWSTR) wszPathName,
			    (int) sizeof( wszPathName ) ) == 0)
	{
	    return MQ_ERROR_INVALID_PARAMETER;
	}

    if (len < sizeof( wszPathName ) )
	wszPathName[len]= 0; // need this to terminate

    hr = MQDeleteQueue( wszPathName );

    return hr;
};



HRESULT MsmqQueue::openQueue(char *szQueuePath, int openmode)
{

    // DIRECT=OS: says to use the computer name to identify the message queue
    // dinoch - Mon, 18 Apr 2005  16:25
    // removed initialization
    //CHAR szDestFormatName[MQ_MAX_Q_NAME_LEN] = "DIRECT=OS:";
    CHAR szDestFormatName[MQ_MAX_Q_NAME_LEN];
    WCHAR wszDestFormatName[2*MQ_MAX_Q_NAME_LEN];
    long	accessmode = openmode;  // bit field: MQ_{RECEIVE,SEND,PEEK,ADMIN}_ACCESS, 
    long	sharemode = MQ_DENY_NONE;

    // Validate the input string.
    if (szQueuePath == NULL)
	{
	    return MQ_ERROR_INVALID_PARAMETER;
	}


    // dinoch - Mon, 18 Apr 2005  16:24
    // removed.  The caller now needs to add any required prefix. 
    // add a prefix of DIRECT=OS: to MSMQ queuepath
    //strcat(szDestFormatName, szQueuePath);

    strncpy(szDestFormatName, szQueuePath, sizeof(szDestFormatName)/sizeof(CHAR));

    // convert to wide characters;
    if (MultiByteToWideChar(
			    (UINT) CP_ACP,
			    (DWORD) 0,
			    (LPCSTR) szDestFormatName,
			    (int) sizeof(szDestFormatName),
			    (LPWSTR) wszDestFormatName,
			    (int) sizeof(wszDestFormatName) ) == 0)
	{
	    return MQ_ERROR_INVALID_PARAMETER;
	}

    HRESULT hr = MQ_OK;

    // dinoch Mon, 18 Apr 2005  16:12

      printf("open: ");
      printf("fmtname(%ls) ", wszDestFormatName);
      printf("accessmode(%d) ", accessmode);
      printf("sharemode(%d) ", sharemode);
     printf("\n");

    hr = MQOpenQueue(
		     wszDestFormatName,           // Format name of the queue
		     accessmode,		  // Access mode
		     sharemode,  	          // Share mode
		     &hQueue   	                  // OUT: Handle to queue
		     );

    // Retry to handle AD replication delays. 
    //
    if (hr == MQ_ERROR_QUEUE_NOT_FOUND)
	{
	    int iCount = 0 ;
	    while((hr == MQ_ERROR_QUEUE_NOT_FOUND) && (iCount < 120))
		{
		    printf(".");

		    // Wait a bit.
		    iCount++ ;
		    Sleep(50);

		    // Retry.
		    hr = MQOpenQueue(wszDestFormatName, 
				     accessmode,
				     sharemode,
				     &hQueue);
		}
	}

    if (FAILED(hr))
	{
	    MQCloseQueue(hQueue);
	}

    return hr;

};







HRESULT MsmqQueue::read(
                             char    *szMessageBody,
                             int     iMessageBodySize,
                             char    *szCorrelationID,
                             char    *szLabel,
			     int     timeout, 
			     int     ReadOrPeek // 1== READ, 0 == Peek
                             )
{
    const int NUMBEROFPROPERTIES = 8;
    DWORD i = 0;
    HRESULT hr = MQ_OK;

    DWORD dwAction= (ReadOrPeek==1)? MQ_ACTION_RECEIVE : MQ_ACTION_PEEK_CURRENT;

    MQMSGPROPS msgprops;
    MSGPROPID aMsgPropId[NUMBEROFPROPERTIES];
    PROPVARIANT aMsgPropVar[NUMBEROFPROPERTIES];

    aMsgPropId[i] = PROPID_M_BODY;
    aMsgPropVar[i].vt = VT_VECTOR | VT_UI1;
    aMsgPropVar[i].caub.pElems = (LPBYTE)szMessageBody;
    aMsgPropVar[i].caub.cElems = iMessageBodySize+32;
    i++;

    aMsgPropId[i] = PROPID_M_CORRELATIONID;
    aMsgPropVar[i].vt = VT_VECTOR | VT_UI1;
    aMsgPropVar[i].caub.pElems = (LPBYTE)szCorrelationID;
    aMsgPropVar[i].caub.cElems = PROPID_M_CORRELATIONID_SIZE;
    i++;

    aMsgPropId[i] = PROPID_M_LABEL_LEN;            // Property ID
    aMsgPropVar[i].vt =VT_UI4;                     // Type indicator
    aMsgPropVar[i].ulVal = MQ_MAX_MSG_LABEL_LEN;   // Label buffer size
    i++;

    WCHAR wszLabelBuffer[MQ_MAX_MSG_LABEL_LEN];
    aMsgPropId[i] = PROPID_M_LABEL;
    aMsgPropVar[i].vt = VT_LPWSTR;
    aMsgPropVar[i].pwszVal = wszLabelBuffer;
    i++;

    msgprops.cProp = i;                         // Number of message properties
    msgprops.aPropID = aMsgPropId;                    // IDs of the message properties
    msgprops.aPropVar = aMsgPropVar;                  // Values of the message properties
    msgprops.aStatus  = 0;			  // Error reports

    // clear out the receiving buffers
    memset(szMessageBody, 0, iMessageBodySize);
    memset(szLabel, 0, MQ_MAX_MSG_LABEL_LEN);
    memset(szCorrelationID, 0, PROPID_M_CORRELATIONID_SIZE);

    hr = MQReceiveMessage(
			  hQueue,             // Handle to the destination queue
			  timeout,	      // Time out interval
			  dwAction,           // Peek?  or Dequeue.  Receive action
			  &msgprops,          // Pointer to the MQMSGPROPS structure
			  NULL, NULL, NULL,   // No OVERLAPPED structure etc.
			  MQ_NO_TRANSACTION  // MQ_SINGLE_MESSAGE | MQ_MTS_TRANSACTION |
			                     // MQ_XA_TRANSACTION   
			  ); 

    if (hr==0) {
	// http://msdn.microsoft.com/library/en-us/intl/unicode_2bj9.asp
	if (0 != WideCharToMultiByte(
				     (UINT) CP_ACP,                // code page
				     (DWORD) 0,                    // conversion flags
				     (LPCWSTR) wszLabelBuffer,     // wide-character string to convert
				     (int) wcslen(wszLabelBuffer), // number of chars in string.
				     (LPSTR) szLabel,                // buffer for new string
				     MQ_MAX_MSG_LABEL_LEN,         // size of buffer
				     (LPCSTR) NULL,                // default for unmappable chars
				     (LPBOOL) NULL                 // set when default char used
				     )) {
	    // actually converted, so... we are happy...
	}
    }

    return hr;
};




HRESULT MsmqQueue::sendMessage(
                             char    *szMessageBody,
                             int     iMessageBodySize,
                             char    *szLabel,
                             char    *szCorrelationID,
                             int     transactionFlag
                             )
{
    const int NUMBEROFPROPERTIES = 3;                 // Number of properties
    DWORD i = 0;
    HRESULT hr = MQ_OK;
    CHAR  aszCorrelationID[PROPID_M_CORRELATIONID_SIZE+1];

    int len;

    len= strlen(szCorrelationID); 
    if (len > 0) {
	if (len > PROPID_M_CORRELATIONID_SIZE) len= PROPID_M_CORRELATIONID_SIZE;
	// copy across JMS correlationID truncating to MSMQ msgid size of 20
	memset(aszCorrelationID, 0, PROPID_M_CORRELATIONID_SIZE+1);
	strncpy(aszCorrelationID, szCorrelationID, len);
    }

    // Define an MQMSGPROPS structure.
    MQMSGPROPS msgprops;
    MSGPROPID aMsgPropId[NUMBEROFPROPERTIES];
    PROPVARIANT aMsgPropVar[NUMBEROFPROPERTIES];
    HRESULT aMsgStatus[NUMBEROFPROPERTIES];

//     char * szFormattedMessageBody = (char *) malloc(iMessageBodySize + 16);
//     if (szFormattedMessageBody == NULL)
// 	{
// 	    return -1;
// 	}

//     memset(szFormattedMessageBody, 0, iMessageBodySize + 1);
//     memcpy(szFormattedMessageBody, szMessageBody, iMessageBodySize);

    aMsgPropId[i] = PROPID_M_BODY;
    aMsgPropVar[i].vt = VT_VECTOR | VT_UI1;
    aMsgPropVar[i].caub.pElems = (LPBYTE)szMessageBody; //szFormattedMessageBody
    aMsgPropVar[i].caub.cElems = iMessageBodySize ;
    i++;

    aMsgPropId[i] = PROPID_M_CORRELATIONID;
    aMsgPropVar[i].vt = VT_VECTOR | VT_UI1;
    aMsgPropVar[i].caub.pElems = (LPBYTE)aszCorrelationID;
    aMsgPropVar[i].caub.cElems = PROPID_M_CORRELATIONID_SIZE;
    i++;

    aMsgPropId[i] = PROPID_M_LABEL;
    aMsgPropVar[i].vt = VT_LPWSTR;

    WCHAR wszLabel[100] = L"Message Label";
    len= strlen(szLabel);
    if (len > 0) {
	    if (MultiByteToWideChar(
				    (UINT) CP_ACP,
				    (DWORD) 0,
				    (LPCSTR) szLabel,
				    strlen(szLabel) ,
				    (LPWSTR) wszLabel,
				    (int) sizeof(wszLabel) ) == 0)
		{
		    return MQ_ERROR_INVALID_PARAMETER;
		}
	    if (len < sizeof(wszLabel) ) wszLabel[len]=0; // terminate
	}
    aMsgPropVar[i].pwszVal = wszLabel;
    i++;

    // Initialize the MQMSGPROPS structure.
    msgprops.cProp = i;                         // Number of message properties
    msgprops.aPropID = aMsgPropId;                    // IDs of the message properties
    msgprops.aPropVar = aMsgPropVar;                  // Values of the message properties
    msgprops.aStatus  = aMsgStatus;                   // Error reports


    // Call MQSendMessage to put the message to the queue. 
    hr = MQSendMessage(
		       hQueue,             // Handle to the destination queue
		       &msgprops,          // Pointer to the MQMSGPROPS structure
		       (ITransaction *) transactionFlag   
		       );                  

    // transactionFlag:  MQ_NO_TRANSACTION, MQ_MTS_TRANSACTION, MQ_XA_TRANSACTION, or MQ_SINGLE_MESSAGE 
    // see mq.h for details...

    return hr;
};


HRESULT MsmqQueue::closeQueue( )
{
    HRESULT hr = MQ_OK;
    hr = MQCloseQueue(hQueue);
    return hr;
};


