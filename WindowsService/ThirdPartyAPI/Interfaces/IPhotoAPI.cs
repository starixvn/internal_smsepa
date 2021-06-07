namespace SMS.EPA.WindowsService.ThirdPartyAPI
{
    using Core.Model;
    using EPA.Model.API;
    using Model;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IPhotoAPI
    {
        Task<DateTime> GetLastModifiedDateAsync(TokenAccess token);

        Task<CommandResult> Update(IEnumerable<Photo> photos, TokenAccess token);
    }
}