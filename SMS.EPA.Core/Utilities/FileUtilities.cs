namespace SMS.EPA.Core.Utilities
{
    using System.IO;
    using System.Threading;

    public class FileUtilities
    {
        public static bool IsAvailable(string path)
        {
            try
            {
                using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    return true;
                }
            }
            catch (IOException)
            {
                return false;
            }
        }

        public static bool IsReadyWithIntervals(string pdfFile, int seconds)
        {
            for (int count = 0; count < seconds; count++)
            {
                var isAvailable = FileUtilities.IsAvailable(pdfFile);
                var isExisted = File.Exists(pdfFile);
                if (isAvailable && isExisted)
                {
                    return true;
                }

                Thread.Sleep(1000);
            }

            return false;
        }

        public static byte[] ReadAndDeleteFile(string path)
        {
            var file = File.ReadAllBytes(path);
            File.Delete(path);

            return file;
        }
    }
}