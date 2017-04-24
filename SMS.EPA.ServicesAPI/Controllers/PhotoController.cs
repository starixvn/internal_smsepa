namespace SMS.EPA.ServicesAPI.Controllers
{
    using Fanex.Logging;
    using Model;
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Web.Http;
    using Utilities;

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
                // -2: for get different GMT+7 and GMT+8
                lastModifiedDate = lastModifiedFile.LastWriteTime.AddHours(-2);
            }

            return Ok(lastModifiedDate);
        }

        public IHttpActionResult Update(Photo[] photos)
        {
            LogService.Info(photos.Length.ToString());
            foreach (Photo photo in photos)
            {
                var fullPath = AppConfig.PhotoPath + "\\" + photo.Code;
                File.WriteAllBytes(fullPath, photo.PhotoData);
            }

            return StatusCode(HttpStatusCode.OK);
        }
    }
}
