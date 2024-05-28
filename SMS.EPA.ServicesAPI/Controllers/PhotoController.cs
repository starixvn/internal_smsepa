namespace SMS.EPA.ServicesAPI.Controllers
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Web.Http;
    using Model.API;
    using Utilities;

    [Authorize]
    public class PhotoController : ApiController
    {
        public IHttpActionResult GetLastModifiedDate()
        {
            var lastModifiedDate = new DateTimeOffset(2016, 1, 1, 0, 0, 0, TimeSpan.Zero).Date;

            var directory = new DirectoryInfo(AppConfig.PhotoPath);
            var lastModifiedFile = directory.GetFiles().OrderByDescending(f => f.LastWriteTime).FirstOrDefault();

            if (lastModifiedFile != null)
            {
                // -2: for get different GMT+7 and GMT+8
                const int diffFromGMT7AndGMT8 = -2;
                lastModifiedDate = lastModifiedFile.LastWriteTime.AddHours(diffFromGMT7AndGMT8);
            }

            return Ok(lastModifiedDate);
        }

        [HttpPost]
        public IHttpActionResult Update(Photo[] photos)
        {
            foreach (var photo in photos)
            {
                var fullPath = string.Format("{0}\\{1}", AppConfig.PhotoPath, photo.Code);
                File.WriteAllBytes(fullPath, photo.PhotoData);
            }

            return StatusCode(HttpStatusCode.OK);
        }
    }
}