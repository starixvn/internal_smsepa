namespace SMS.EPA.WindowsService.Services
{
    using Core.Model;
    using EPA.Model.API;
    using Model;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IPhotoService
    {
        IEnumerable<Photo> GetPhotos(DateTime? fromDate);

        Task<DateTime> GetLastModifiedDate(TokenAccess token);

        Task<CommandResult> UpdateAsync(IEnumerable<Photo> photos, TokenAccess token);
    }
}