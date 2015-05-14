using System.IO;

namespace Logictracker.Scheduler.Tasks.ControlAcceso
{
    public static class StreamExtensions
    {
        const int BufferSize = 4096;

        public static void CopyTo(this Stream input, Stream output)
        {

            var buffer = new byte[BufferSize];
            while (true)
            {
                var read = input.Read(buffer, 0, buffer.Length);
                if (read <= 0) return;
                output.Write(buffer, 0, read);
            }
        }
    }
}