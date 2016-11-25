namespace SMS.EPA.ClientHrm.Services
{
    using System;
    using System.Collections.Generic;
    using SMS.EPA.ClientHrm.Model;

    public interface IPhotoService
    {
        IEnumerable<Photo> GetPhotos(DateTime? fromDate);

        DateTime GetLastModifiedDate(TokenAccess token);

        CommandResult Update(IEnumerable<Photo> photos, TokenAccess token);
    }
}