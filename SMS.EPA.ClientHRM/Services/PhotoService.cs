namespace SMS.EPA.ClientHRM.Services
{
    using Fanex.Data.Repository;
    using SMS.EPA.ClientHRM.Model;
    using SMS.EPA.ClientHRM.ThirdPartyAPIs;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class PhotoService : IPhotoService
    {
        private readonly IDynamicRepository _dynamicRepository;
        private readonly IPhotoAPI _photoAPI;

        public PhotoService(IDynamicRepository dynamicRepository, IPhotoAPI photoAPI)
        {
            _dynamicRepository = dynamicRepository;
            _photoAPI = photoAPI;
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

        public DateTime GetMaxModifiedDate(TokenAccess token)
        {
            return _photoAPI.GetMaxModifiedDate(token);
        }

        public CommandResult Update(IEnumerable<Photo> photos, TokenAccess token)
        {
            return _photoAPI.Update(photos, token);
        }
    }
}
