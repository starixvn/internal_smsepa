namespace SMS.EPA.ClientHrm.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Model;
    using EPA.Model.API;
    using Model;
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
            var directory = new DirectoryInfo(ApplicationConfig.PhotoPath);
            var modifiedDate = fromDate ?? new DateTimeOffset(2010, 1, 1, 0, 0, 0, TimeSpan.Zero).DateTime;
            IEnumerable<FileInfo> files = directory.GetFiles().Where(file => file.LastWriteTime >= modifiedDate
                && file.Extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase)).OrderBy(file => file.LastWriteTime).ToList();

            foreach (FileInfo file in files)
            {
                photos.Add(
                    new Photo
                    {
                        Code = file.Name,
                        PhotoData = File.ReadAllBytes(file.FullName)
                    });
            }

            return photos;
        }

        public async Task<DateTime> GetLastModifiedDate(TokenAccess token)
        {
            return await _photoApi.GetLastModifiedDateAsync(token);
        }

        public async Task<CommandResult> UpdateAsync(IEnumerable<Photo> photos, TokenAccess token)
        {
            return await _photoApi.Update(photos, token);
        }
    }
}