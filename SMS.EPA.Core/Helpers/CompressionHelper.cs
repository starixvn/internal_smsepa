namespace SMS.EPA.Core.Helpers
{
    using System.IO;
    using Ionic.Zlib;

    public static class CompressionHelper
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "Reviewed")]
        public static byte[] DeflateByte(byte[] str)
        {
            if (str == null)
            {
                return null;
            }

            using (var output = new MemoryStream())
            {
                using (var streamCompressor =
                    new DeflateStream(output, CompressionMode.Compress, CompressionLevel.BestSpeed))
                {
                    streamCompressor.Write(str, 0, str.Length);
                }

                return output.ToArray();
            }
        }
    }
}