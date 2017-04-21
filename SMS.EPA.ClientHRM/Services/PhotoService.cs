namespace SMS.EPA.ClientHrm.Services
{
    using Model;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using ThirdPartyAPI;

    public class PhotoService : IPhotoService
    {
        private readonly IPhotoAPI _photoApi;

        public PhotoService(IPhotoAPI photoApi)
        {
            _photoApi = photoApi;
        }

        public IEnumerable<Photo> GetPhotos(DateTime? fromDate)
        {
            var photos = new List<Photo>();
            DirectoryInfo directory = new DirectoryInfo(ApplicationConfig.PhotoPath);
            DateTime modifiedDate = fromDate ?? new DateTime(2010, 1, 1);
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
            return _photoApi.GetLastModifiedDate(token);
        }

        public CommandResult Update(IEnumerable<Photo> photos, TokenAccess token)
        {
            return _photoApi.Update(photos, token);
        }
    }
}