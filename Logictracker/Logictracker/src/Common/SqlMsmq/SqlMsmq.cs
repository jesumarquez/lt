using System;
using Microsoft.SqlServer.Server;
using System.Messaging;
using System.IO;

namespace SqlMsmq
{
    public class SqlMsmq
    {  
        [SqlProcedure]
        public static void SendMessage(string queuePath,string logPath, string messageLabel, string messageBody)
        {
            var fileName = logPath;

            var writer = File.AppendText(fileName);

            try
            {
                var mq = new MessageQueue(queuePath)
                             {
                                 Formatter = new BinaryMessageFormatter(),
                                 DefaultPropertiesToSend = new DefaultPropertiesToSend {Recoverable = true}
                             };

                var mm = new Message
                             {
                                 Label = messageLabel,
                                 Body = messageBody,
                                 Formatter = new BinaryMessageFormatter()
                             };

                mq.Send(mm);

                writer.Write(string.Concat("INFO: Se insertó el mensaje ", messageLabel, ":", messageBody,
                    " en la cola ", queuePath, Environment.NewLine));

                mq.Close();
           }
           catch (Exception e) { writer.Write(string.Concat("ERROR: ", e.Message, Environment.NewLine)); }
           finally { writer.Close(); }
        }

        //[SqlProcedure]
        //public static void SendGenericMessage(string queuePath,string messageLabel,string messageBody)
        //{
        //    var fileName = "C:\\MessageLogQueue.txt";

        //    var writer = File.AppendText(fileName);

        //    var msgBody = ParseMessage(messageLabel, messageBody);
        //    try
        //    {
        //        var mq = new MessageQueue(queuePath)
        //        {
        //            Formatter = new BinaryMessageFormatter(),
        //            DefaultPropertiesToSend = new DefaultPropertiesToSend { Recoverable = true }
        //        };

        //        var mm = new Message
        //        {
        //            Label = messageLabel,
        //            Body = msgBody,
        //            Formatter = new BinaryMessageFormatter()
        //        };

        //        mq.Send(mm);

        //        writer.Write(string.Concat("INFO: Se insertó el mensaje ", messageLabel, ":", messageBody,
        //            " en la cola ", queuePath, Environment.NewLine));

        //        mq.Close();
        //    }
        //    catch (Exception e) { writer.Write(string.Concat("ERROR: ", e.Message, Environment.NewLine)); }
        //    finally { writer.Close(); }
        //}

        ///// <summary>
        ///// Label = CodigoDeMensaje , Body = IdMensaje en base
        ///// </summary>
        ///// <param name="label"></param>
        ///// <param name="body"></param>
        ///// <returns></returns>
        //private static Generico ParseMessage(string label, string body)
        //{
        //    return new Generico {IdMsg = Convert.ToInt32(body), Evento = Convert.ToByte(label)};
        //}
    }
}
