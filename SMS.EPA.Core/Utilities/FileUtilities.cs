using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace SMS.EPA.Core.Utilities
{
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
            var isExisted = false;
            var isAvailable = false;
            int count = 0;
            while (!isExisted && count < seconds)
            {
                isAvailable = FileUtilities.IsAvailable(pdfFile);
                isExisted = System.IO.File.Exists(pdfFile);
                count++;

                Thread.Sleep(1000);
            }

            return isExisted && isAvailable && count < seconds;
        }

        public static byte[] ReadAndDeleteFile(string path)
        {
            var file = System.IO.File.ReadAllBytes(path);
            System.IO.File.Delete(path);

            return file;
        }
    }
}
