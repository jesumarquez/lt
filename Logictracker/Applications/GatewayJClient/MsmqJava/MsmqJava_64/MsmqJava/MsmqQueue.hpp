
class MsmqQueue
{
	private:
		QUEUEHANDLE		hQueue;

	public:

		HRESULT createQueue(
                             char    *szQueuePath,
                             char    *szQueueLabel,
			     LPWSTR  wszFormatName, 
			     DWORD   * p_dwFormatNameBufferLength,
                             int     isTransactional
                             );

		HRESULT deleteQueue(
                             char    *szQueuePath
			     );

		HRESULT openQueue(
                             char    *szMSMQQueuePath,
                             int	 openmode
                             );

		HRESULT read(
                             char    *szMessageBody,
                             int     iMessageBodySize,
                             char    *szCorrelationID,
                             char    *szLabel,
                             int     timeout, 
			     int     ReadOrPeek
                             );

		HRESULT sendMessage(
                             char    *szMessageBody,
                             int     iMessageBodySize,
                             char    *szLabel,
                             char    *szCorrelationID,
			     int     transactionFlag
                             );

		HRESULT closeQueue( void );

};
