namespace SMS.EPA.WindowsService.Services
{
    using Core.Model;
    using EPA.Model.API;
    using Model;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using ThirdPartyAPI;

    public class PhotoService : IPhotoService
    {
        private readonly IPhotoAPI photoApi;

        public PhotoService(IPhotoAPI photoApi)
        {
            this.photoApi = photoApi;
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
            return await photoApi.GetLastModifiedDateAsync(token);
        }

        public async Task<CommandResult> UpdateAsync(IEnumerable<Photo> photos, TokenAccess token)
        {
            return await photoApi.Update(photos, token);
        }
    }
}