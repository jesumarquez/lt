#region Usings

using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Urbetrack.Configuration;
using Urbetrack.DatabaseTracer.Core;

#endregion

namespace Urbetrack.Backbone
{
    public static class ObjectsStorage
    {
        public static String DatabasePath { get; set; }
        public static bool Running { get; private set; }

        static ObjectsStorage()
        {
            DatabasePath = Config.Torino.StorageDatabasePath;
            Running = true;
        }

        public static void Shutdown()
        {
            STrace.Debug(typeof(ObjectsStorage).FullName, "OBJECTS STORAGE: Shutdown!");
            Running = false;
        }

        private static readonly object locker = new object();

        public static bool Store<_Tclass>(string object_name, _Tclass data)
        {
            if (!Running)
            {
                STrace.Debug(typeof(ObjectsStorage).FullName, String.Format("OBJECTS STORAGE: el objeto {0} de tipo {1} no se grabara (el storage esta shutdown)", object_name, typeof(_Tclass).FullName));
                return false;
            }
            try
            {
                lock (locker)
                {
                    var datatype = typeof(_Tclass).FullName;
                    IFormatter formatter = new BinaryFormatter();
                    var filepath = String.Format("{0}\\{1}", DatabasePath, datatype);
                    if (!Directory.Exists(filepath)) {
                        Directory.CreateDirectory(filepath);
                    }
                    var filename = String.Format("{0}\\{1}.object", filepath, object_name);                    
                    Stream stream = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
                    stream.SetLength(0);
                    formatter.Serialize(stream, data);
                    stream.Close(); // commit!
                    return true;
                }
            } catch (Exception e)
            {
                STrace.Exception(typeof(ObjectsStorage).FullName,e, String.Format("Storage.Store({0})", object_name));
                return false;
            }
        }

        public static _Tclass Load<_Tclass>(string object_name)
        {
            try
            {
                if (!Running)
                {
                    STrace.Debug(typeof(ObjectsStorage).FullName, String.Format("OBJECTS STORAGE: el objeto {0} de tipo {1} no se leera de la base (el storage esta shutdown)", object_name, typeof(_Tclass).FullName));
                    return default(_Tclass);
                }
                lock (locker)
                {
                    var datatype = typeof(_Tclass).FullName;
                    IFormatter formatter = new BinaryFormatter();
                    var filepath = String.Format("{0}\\{1}", DatabasePath, datatype);
                    if (!Directory.Exists(filepath))
                    {
                        Directory.CreateDirectory(filepath);
                    }
                    var filename = String.Format("{0}\\{1}.object", filepath, object_name);
                    Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                    var obj = (_Tclass)formatter.Deserialize(stream);
                    stream.Close();
                    return obj;
                }
            }
            catch (FileNotFoundException )
            {
                return default(_Tclass);
            }
            catch (Exception e)
            {
                STrace.Exception(typeof(ObjectsStorage).FullName,e, String.Format("Storage.Load({0})", object_name));
                var obj = default(_Tclass);
                return obj;
            }
        }

    }
}