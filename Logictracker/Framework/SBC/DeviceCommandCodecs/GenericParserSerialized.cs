using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;
using System.Reflection;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Model;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Utils;
using System.Runtime.InteropServices;

namespace Logictracker.Layers.DeviceCommandCodecs
{
    public class GenericParserSerialized
    {
        public virtual void readArrayPacketWithSplit(byte[] Arraystream, object objectSerialized, string[] splitFormats)
        {
            using (StreamReader streamReader = new StreamReader(new MemoryStream(Arraystream)))
            {
                int index = 0;
                string[] fields = streamReader.ReadToEnd().Split(splitFormats, StringSplitOptions.None);
                foreach (PropertyInfo propInfo in objectSerialized.GetType().GetProperties())
                {
                    //string name = propInfo.Name;
                    // Type type = propInfo.PropertyType;
                    if (fields.Length > 0 &&
                        index < fields.Length)
                    {
                        propInfo.SetValue(objectSerialized, fields[index], null);
                        index++;
                    }
                    else
                        break;
                }
            }
        }
        public virtual Object getStructData(byte[] array, Object final)
        {
            IntPtr pBuf = Marshal.StringToBSTR(System.Text.Encoding.Default.GetString(array));
            Object ms = (Object)Marshal.PtrToStructure(pBuf, final.GetType());
            Marshal.FreeBSTR(pBuf);
            return ms;
        }
    }
}
        








    

       

    

