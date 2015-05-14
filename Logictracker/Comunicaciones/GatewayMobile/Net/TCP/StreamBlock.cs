using System;

namespace Urbetrack.Mobile.Net.TCP
{
    public class StreamBlock
    {
        public enum ContentTypes
        {
            Disconnect,
            StreamData,
            OOBData
        };

        public StreamBlock()
        {
            UniqueIdentifier = Guid.NewGuid();
        }

        public Guid UniqueIdentifier { get; private set; }

        public ContentTypes ContentType { get; set; }

        public int TotalBytes { get; set; }

        public byte[] Data { get; set; }

        public override bool Equals(object obj)
        {
            var par = (StreamBlock) obj;
            return par.UniqueIdentifier == UniqueIdentifier;
        }

        public override int GetHashCode()
        {
            return UniqueIdentifier.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("{0}",ContentType);
        }
    }
}