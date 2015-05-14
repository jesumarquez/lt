package ionic.Msmq;

public class Queue
{
  public Queue(String queueName) throws  MessageQueueException
  {
    _init(queueName, 0x03);  // open with both SEND and RECEIVE access
  }

  public Queue(String queueName, int access) throws  MessageQueueException
  {
    _init(queueName, access);
  }

  void _init(String queueName, int access) throws  MessageQueueException
  {
    int rc = 0;
    if (access == 0x01) // RECEIVE
    {
     rc= nativeOpenQueueForReceive(queueName);
    }
    else
    if (access == 0x02) // SEND
    {
     rc= nativeOpenQueueForSend(queueName);
    }
    else
    if (access == 0x03) // SEND+RECEIVE
    {
     rc= nativeOpenQueue(queueName);
    }
    else { rc= 0xC00E0006; /* MQ_INVALID_PARAMETER */ }

    if (rc!=0) throw new  MessageQueueException("Cannot open queue.", rc);

    _name= queueName;
    _formatName= "unknown";
    _label= "need to set this";
    _isTransactional= false; // TODO: get actual value in "openQueue"
  }

  public static Queue create(String queuePath, String queueLabel, boolean isTransactional) throws  MessageQueueException
  {
    int rc= nativeCreateQueue( queuePath,  queueLabel,  (isTransactional)?1:0);
    if (rc!=0)
      throw new  MessageQueueException("Cannot create queue.", rc);
    // DIRECT=OS  ?  or DIRECT=TCP ?
    String a1= "OS";
    char[] c= queuePath.toCharArray();
    if ((c[0]>='1') && (c[0]<='9'))
      a1= "TCP"; // assume ip address

    Queue q= new Queue("DIRECT=" + a1 + ":" + queuePath);
    q._name= queuePath;
    q._label=queueLabel;
    q._isTransactional= isTransactional;
    return q;
  }

  public static void delete(String queuePath) throws  MessageQueueException
  {
    int rc= nativeDeleteQueue( queuePath );
    if (rc!=0)
      throw new  MessageQueueException("Cannot delete queue.", rc);
  }


  public void send(Message msg) throws  MessageQueueException
  {
    int rc= nativeSend(msg.getMessage(),
		       msg.getMessage().length(),
		       msg.getLabel(),
		       msg.getCorrelationId(),
		       msg.getTransactionFlag());
    if (rc!=0)
      throw new MessageQueueException("Cannot send.", rc);
  }

  /*public void send(String s) throws  MessageQueueException
  {
    int rc= nativeSend(s,
		       s.length(),
		       "", // empty label
		       "", // empty correlationId
		       0  // outside any transaction
		       );
    if (rc!=0)
      throw new MessageQueueException("Cannot send.", rc);
  }//*/

  private Message receiveEx(int timeout, int ReadOrPeek) throws  MessageQueueException
  {
    int rc = nativeReceive(128, timeout, ReadOrPeek);

    if (rc!=0)
      throw new MessageQueueException("Cannot receive.", rc);

    return new Message(_lastMessageRetrieved_MessageString,
		       _lastMessageRetrieved_MessageLabel,
		       _lastMessageRetrieved_CorrelationId,
		       0);
  }

  public Message receive(int timeout) throws  MessageQueueException
  {
    return receiveEx(timeout, 1);
  }

  public Message receive() throws  MessageQueueException
  {
    return receiveEx(0,1); // infinite timeout
  }

  public Message peek() throws  MessageQueueException
  {
    return receiveEx(0,0); // infinite timeout
  }

  public Message peek(int timeout) throws  MessageQueueException
  {
    return receiveEx(timeout,0);
  }

  public void close() throws  MessageQueueException
  {

    int rc=nativeClose();
    if (rc!=0)
      throw new MessageQueueException("Cannot close.", rc);
  }

  public String getName(){ return _name; }
  public String getLabel(){ return _label; }
  public String getFormatName(){ return _formatName; }
  public boolean isTransactional(){ return _isTransactional; }

  private static native int nativeInit();
  private static native int nativeCreateQueue(String queuePath, String queueLabel, int isTransactional);
  private static native int nativeDeleteQueue(String queuePath);
  private native int nativeOpenQueue(String queueString);
  private native int nativeOpenQueueForSend(String queueString);
  private native int nativeOpenQueueForReceive(String queueString);
  private native int nativeReceive(int length, int timeout, int ReadOrPeek);
  private native int nativeSend(String messageString, int length, String label, String correlationString, int transactionFlag);
  private native int nativeClose();

  int	_queueSlot = 0;
  String _name;
  String _formatName;
  String _label;
  boolean _isTransactional;

  String _lastMessageRetrieved_MessageString;
  String _lastMessageRetrieved_MessageLabel;
  String _lastMessageRetrieved_CorrelationId;

  static {
    System.loadLibrary("MsmqJava");
    nativeInit();
  }
}
