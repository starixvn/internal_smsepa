namespace SMS.EPA.ClientHRM.ThirdPartyAPIs
{
    using SMS.EPA.ClientHRM.Model;
    using System;
    using System.Collections.Generic;

    public interface IPhotoAPI
    {
        DateTime GetLastModifiedDate(TokenAccess token);

        CommandResult Update(IEnumerable<Photo> photos, TokenAccess token);
    }
}
