namespace SMS.EPA.ClientHrm.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Model;
    using EPA.Model.API;
    using Model;

    public interface IPhotoService
    {
        IEnumerable<Photo> GetPhotos(DateTime? fromDate);

        Task<DateTime> GetLastModifiedDate(TokenAccess token);

        Task<CommandResult> UpdateAsync(IEnumerable<Photo> photos, TokenAccess token);
    }
}