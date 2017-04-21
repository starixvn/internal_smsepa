namespace SMS.EPA.ClientHrm.Services
{
    using Model;
    using System;
    using System.Collections.Generic;

    public interface IPhotoService
    {
        IEnumerable<Photo> GetPhotos(DateTime? fromDate);

        DateTime GetLastModifiedDate(TokenAccess token);

        CommandResult Update(IEnumerable<Photo> photos, TokenAccess token);
    }
}