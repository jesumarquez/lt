using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Messaging;
using System.Runtime.InteropServices;

namespace Urbetrack.FleetManagment.MQ
{
    public class MQueue
    {
        private string _name;
        private System.Messaging.MessageQueue queue;

        public MQueue(String name)
        {
            _name = name;
            OpenQueue();
        }

        public void Send(string commandText)
        {
            Send(commandText, commandText.Length < 200 ? commandText : commandText.Substring(0, 200));
        }

        public Boolean Send(Object message, String label)
        {
            var msgsnd = new Message(message) { Label = label };
            Send(msgsnd);
            return true;
        }

        public Boolean Send(Message msgsnd)
        {
            if (queue == null) return false;
            msgsnd.Formatter = queue.Formatter;
            queue.Send(msgsnd);
            return true;
        }


        public bool OpenQueue()
        {
            try
            {
                if (MessageQueue.Exists(_name))
                {
                    queue = new MessageQueue(_name);
                }
                else
                {
                    queue = MessageQueue.Create(_name);
                }

                queue.SetPermissions(GetUsersGroupLocalizedName(), MessageQueueAccessRights.FullControl);
                queue.Formatter = new BinaryMessageFormatter();
                return true;
            }
            catch (Exception e)
            {
                queue = null;
                return false;
            }
        }

        private static String GetUsersGroupLocalizedName()
        {
            IntPtr psid;
            int use;
            var cbName = 0;
            var cbDom = 0;

            ConvertStringSidToSid("S-1-5-32-545", out psid);

            LookupAccountSid(null, psid, null, ref cbName, null, ref cbDom, out use);

            var name = new StringBuilder(cbName);
            var dom = new StringBuilder(cbDom);

            LookupAccountSid(null, psid, name, ref cbName, dom, ref cbDom, out use);

            Marshal.FreeHGlobal(psid);

            return name.ToString();
        }


        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern Boolean ConvertStringSidToSid(String stringSid, out IntPtr psid);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern Boolean LookupAccountSid(String lpSystemName, IntPtr sid, [Out] StringBuilder name, ref Int32 cbName, [Out] StringBuilder referencedDomainName, ref Int32 cbReferencedDomainName, out Int32 peUse);
 
    }
}
