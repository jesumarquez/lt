#region Usings

using System;
using System.IO;
using System.Threading;
using Logictracker.DatabaseTracer.Core;

#endregion

namespace Logictracker.QuadTree.Data
{
    #region Public Classes

    /// <summary>
    /// Class for syncronizing file aoperations and providing a red/write cache.
    /// </summary>
    public class FileManager
    {
        #region Private Properties

        /// <summary>
        /// The current file path.
        /// </summary>
        private readonly String _filePath;

        #endregion

        #region Public Properties

        /// <summary>
        /// I/O operations timeout.
        /// </summary>
        public TimeSpan Timeout { get; set; }

        /// <summary>
        /// Timespan for rtying I/O operation.
        /// </summary>
        public TimeSpan Retry { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Instanciates a new file manager with the specified file path and cache mode.
        /// </summary>
        /// <param name="filePath">The path of the file to be managed.</param>
        public FileManager(String filePath)
        {
            _filePath = filePath;

            Timeout = TimeSpan.FromMilliseconds(100);
            Retry = TimeSpan.FromMilliseconds(10);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Reads a byte from the current file.
        /// </summary>
        /// <param name="offset">The offset to read the byte from.</param>
        /// <returns>If posible the byte elsewhere null value.</returns>
        public Byte? ReadByte(long offset)
        {
            var data = ReadRange(offset, 1);

            return data != null ? data[0] : (Byte?) null;
        }

        /// <summary>
        /// Reads a byte array from the current file.
        /// </summary>
        /// <param name="offset">The offset to read the byte array from.</param>
        /// <param name="size">The size of the range to be read.</param>
        /// <returns>If posible the byte array elsewhere null value.</returns>
        public Byte[] ReadRange(long offset, Int32 size)
        {
            try
            {
                var retries = 0;
                var maxRetries = Timeout.TotalMilliseconds / Retry.TotalMilliseconds;
                var readed = false;

                var results = new Byte[size];

                while (!readed && retries <= maxRetries)
                {
                    var file = new FileStream(_filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

                    try
                    {
                        file.Lock(offset, size);

                        file.Seek(offset, SeekOrigin.Begin);

                        file.Read(results, 0, size);

                        file.Unlock(offset, size);

                        readed = true;
                    }
                    catch
                    {
                        Thread.Sleep(Retry);

                        retries++;
                    }
                    finally
                    {
                        file.Close();
                    }
                }

                if (!readed) throw new Exception("Timeout reached for the requested I/O operation.");

                return results;
            }
            catch (Exception e)
            {
				STrace.Exception(typeof(FileManager).FullName, e);

                throw;
            }
        }

        /// <summary>
        /// Writes the specified value into the current file.
        /// </summary>
        /// <param name="offset">The offset to write the data to.</param>
        /// <param name="data">The data to be saved in the file.</param>
        public void WriteByte(long offset, Byte data) { WriteRange(offset, new[]{data}); }

        /// <summary>
        /// Writes the specified value into the current file.
        /// </summary>
        /// <param name="offset">The offset to write the data to.</param>
        /// <param name="data">The data to be saved in the file.</param>
        public void WriteRange(long offset, Byte[] data)
        {
            try
            {
                var retries = 0;
                var maxRetries = Timeout.TotalMilliseconds/Retry.TotalMilliseconds;
                var writed = false;

                while (!writed && retries <= maxRetries)
                {
                    var file = new FileStream(_filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

                    try
                    {
                        file.Lock(offset, data.Length);

                        file.Seek(offset, SeekOrigin.Begin);

                        file.Write(data, 0, data.Length);

                        file.Unlock(offset, data.Length);

                        writed = true;
                    }
                    catch
                    {
                        Thread.Sleep(Retry);

                        retries++;
                    }
                    finally
                    {
                        file.Close();
                    }
                }

                if (!writed) throw new Exception("Timeout reached for the requested I/O operation.");
            }
            catch (Exception e)
            {
				STrace.Exception(typeof(FileManager).FullName, e);

                throw;
            }
        }

        /// <summary>
        /// Creates a new file in the current path with the specified data.
        /// </summary>
        /// <param name="data"></param>
        public void CreateFile(byte[] data)
        {
            try
            {
                var file = new FileStream(_filePath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite);

                file.Write(data, 0, data.Length);

                file.Close();
            }
            catch (Exception e)
            {
				STrace.Exception(typeof(FileManager).FullName, e);

                throw;
            }
        }

        /// <summary>
        /// Creates a new file with the specified amount of sectors.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="sectors"></param>
        public static void CreateFile(string filePath, int sectors)
        {
            var file = new FileStream(filePath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite);
            
            var empty = new byte[512];

            for(var i=0;i<sectors;++i) file.Write(empty, 0, empty.Length);
            
            file.Close();
        }

        #endregion
    }

    #endregion
}
