namespace SMS.EPA.Core.Helpers
{
    using System.IO;

    public static class CompressionHelper
    {
        public static byte[] DeflateByte(byte[] str)
        {
            if (str == null)
            {
                return null;
            }

            using (var output = new MemoryStream())
            {
                var compressor = new Ionic.Zlib.DeflateStream(output, Ionic.Zlib.CompressionMode.Compress, Ionic.Zlib.CompressionLevel.BestSpeed);
                    compressor.Write(str, 0, str.Length);
                    return output.ToArray();
            }
        }
    }
}