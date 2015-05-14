#region Usings

using System;
using System.IO;
using System.Runtime.InteropServices;

#endregion

namespace Logictracker.QuadTree.Data
{
    public class FileMarshal : IDisposable
    {
        private readonly string FileName;
        private Stream FileStream;
        public int CurrentOffset;
        private readonly object LockObject = new object();

        public FileMarshal(string fileName)
        {
            FileName = fileName;
            Open();
        }
        public void Dispose()
        {
            Close();
        }

        private void Open() { 
            //FileStream = File.OpenRead(FileName);
            FileStream = File.Open(FileName, FileMode.Open, FileAccess.ReadWrite, FileShare.Read); 
        }

        private void Close() { FileStream.Close(); }

        private byte[] ReadBytes(int offset, int size)
        {
            var buffer = new byte[size];

            FileStream.Seek(offset, SeekOrigin.Begin);
            FileStream.Read(buffer, 0, size);

            return buffer;
        }


        private T Read<T>(int offset, int size)
        {
            lock (LockObject)
            {
                CurrentOffset = offset + size;
                var buffer = ReadBytes(offset, size);
                unsafe
                {
                    // pin the data array and get a pointer to the first element
                    fixed (byte* ptrBuffer = &buffer[0])
                    {
                        // marshal the structure to the byte array
                        var ms = typeof(T) == typeof(string)
                                        ? Marshal.PtrToStringAnsi(new IntPtr(ptrBuffer))
                                        : Marshal.PtrToStructure(new IntPtr(ptrBuffer), typeof(T));
                        // all done :)
                        return (T)ms;
                    }
                }
            }
        }

        private void Write<T>(int offset, T obj)
        {
            lock (LockObject)
            {
                var data = new byte[512];
                FileStream.Seek(offset, SeekOrigin.Begin);
                unsafe
                {
                    fixed (byte* ptrBuffer = &data[0])
                    {
                        Marshal.StructureToPtr(obj, new IntPtr(ptrBuffer), true);
                    }
                }
                FileStream.Write(data, 0, data.GetLength(0));                                
            }
        }

        public void WriteStruct<T>(T obj) { WriteStruct(CurrentOffset, obj); }
        public void WriteStruct<T>(int offset, T obj) { Write(offset, obj); }

        public T ReadStruct<T>() { return ReadStruct<T>(CurrentOffset); }
        public T ReadStruct<T>(int offset) { return Read<T>(offset, GetSize(typeof(T))); }

        public short ReadInt16() { return ReadInt16(CurrentOffset); }
        public short ReadInt16(int offset) { return ReadStruct<Int16>(offset); }
        public int ReadInt32() { return ReadInt32(CurrentOffset); }
        public int ReadInt32(int offset) { return ReadStruct<Int32>(offset); }
        public long ReadInt64() { return ReadInt64(CurrentOffset); }
        public long ReadInt64(int offset) { return ReadStruct<Int64>(offset); }
        public string ReadString(int size) { return ReadString(CurrentOffset, size); }
        public string ReadString(int offset, int size) { return Read<String>(offset, size); }

        public void SkipStruct<T>() { CurrentOffset += GetSize(typeof(T)); }
        public void SkipInt16() { SkipStruct<Int16>(); }
        public void SkipInt32() { SkipStruct<Int32>(); }
        public void SkipInt64() { SkipStruct<Int64>(); }
        public void Skip(int size) { CurrentOffset += size; }

        public int GetSize(Type T)
        {
            return T == typeof(string) ? 64 : Marshal.SizeOf(T);
        }
    }
}
