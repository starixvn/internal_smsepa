namespace SMS.EPA.ClientHrm.ThirdPartyAPI
{
    using System;
    using System.Collections.Generic;
    using SMS.EPA.ClientHrm.Model;

    public interface IPhotoAPI
    {
        DateTime GetLastModifiedDate(TokenAccess token);

        CommandResult Update(IEnumerable<Photo> photos, TokenAccess token);
    }
}