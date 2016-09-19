namespace SMS.EPA.ServicesAPI.Controllers
{
    using SMS.EPA.ServicesAPI.Model;
    using SMS.EPA.ServicesAPI.Utilities;
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Web.Http;
    using Fanex.Logging;

    [Authorize]
    public class PhotoController : ApiController
    {
        public IHttpActionResult GetLastModifiedDate()
        {
            var lastModifiedDate = new DateTime(2016, 01, 01);

            DirectoryInfo directory = new DirectoryInfo(AppConfig.PhotoPath);
            FileInfo lastModifiedFile = directory.GetFiles().OrderByDescending(f => f.LastWriteTime).FirstOrDefault();

            if (lastModifiedFile != null)
            {
                /// -2: for get different GMT+7 and GMT+8
                lastModifiedDate = lastModifiedFile.LastWriteTime.AddHours(-2);
            }

            return Ok(lastModifiedDate);
        }

        public IHttpActionResult Update(Photo[] photos)
        {
            LogService.Info(photos.Count().ToString());
            string fullPath = string.Empty;
            foreach (Photo photo in photos)
            {
                fullPath = AppConfig.PhotoPath + "\\" + photo.Code;
                File.WriteAllBytes(@fullPath, photo.PhotoData);
            }

            return StatusCode(HttpStatusCode.OK);
        }
    }
}
