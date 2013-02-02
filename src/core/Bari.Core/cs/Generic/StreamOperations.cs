using System.IO;

namespace Bari.Core.Generic
{
    public static class StreamOperations
    {
        /// <summary>
        /// Copies a stream to another one
        /// </summary>
        /// <param name="source">Source stream</param>
        /// <param name="target">Target stream</param>
        public static void Copy(Stream source, Stream target)
        {
            const int localBufferSize = 4096;

            var buf = new byte[localBufferSize];

            int count;
            do
            {
                count = source.Read(buf, 0, localBufferSize);
                target.Write(buf, 0, count);
            } while (count == localBufferSize);
        }
    }
}