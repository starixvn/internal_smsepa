namespace SMS.EPA.ClientHrm.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using SMS.EPA.ClientHrm.Model;
    using SMS.EPA.ClientHrm.ThirdPartyAPIs;

    public class PhotoService : IPhotoService
    {
        private readonly IPhotoAPI photoAPI;

        public PhotoService(IPhotoAPI photoAPI)
        {
            this.photoAPI = photoAPI;
        }

        public IEnumerable<Photo> GetPhotos(DateTime? fromDate)
        {
            var photos = new List<Photo>();
            DirectoryInfo directory = new DirectoryInfo(ApplicationConfig.PhotoPath);
            DateTime modifiedDate = fromDate.HasValue ? fromDate.Value : new DateTime(2010, 1, 1);
            IEnumerable<FileInfo> files = directory.GetFiles().Where(file => file.LastWriteTime >= modifiedDate
                && file.Extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase));

            foreach (FileInfo file in files)
            {
                photos.Add(
                    new Photo()
                    {
                        Code = file.Name,
                        PhotoData = File.ReadAllBytes(file.FullName)
                    });
            }

            return photos;
        }

        public DateTime GetLastModifiedDate(TokenAccess token)
        {
            return photoAPI.GetLastModifiedDate(token);
        }

        public CommandResult Update(IEnumerable<Photo> photos, TokenAccess token)
        {
            return photoAPI.Update(photos, token);
        }
    }
}