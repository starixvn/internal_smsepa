namespace SMS.EPA.ClientHrm.Services
{
    using Core.Model;
    using EPA.Model.API;
    using Model;
    using System;
    using System.Collections.Generic;

    public interface IDisciplineService
    {
        IEnumerable<Discipline> GetDiscipline(DateTime fromDate, DateTime lastModifiedDate);

        DateTime GetLastModifiedDate(TokenAccess token);

        DateTime GetFromDateDiscipline(TokenAccess token);

        void Update(IEnumerable<Discipline> disciplines, TokenAccess token);
    }
}