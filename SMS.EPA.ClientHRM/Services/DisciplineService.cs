namespace SMS.EPA.ClientHrm.Services
{
    using Core.Model;
    using Criteria;
    using EPA.Model.API;
    using Fanex.Data.Repository;
    using Model;
    using System;
    using System.Collections.Generic;
    using ThirdPartyAPI;

    public class DisciplineService : IDisciplineService
    {
        private readonly IDynamicRepository _dynamicRepository;
        private readonly IDisciplineAPI _disciplineApi;

        public DisciplineService(IDynamicRepository dynamicRepository, IDisciplineAPI attendanceApi)
        {
            _dynamicRepository = dynamicRepository;
            _disciplineApi = attendanceApi;
        }

        public IEnumerable<Discipline> GetDiscipline(DateTime fromDate, DateTime lastModifiedDate)
        {
            var criteria = new GetDisciplineCriteria
            {
                FromDate = fromDate,
                LastModifiedDate = lastModifiedDate
            };

            IEnumerable<Discipline> violations = _dynamicRepository.Fetch<Discipline>(criteria);

            return violations;
        }

        public DateTime GetLastModifiedDate(TokenAccess token)
        {
            return _disciplineApi.GetLastModifiedDate(token);
        }

        public DateTime GetFromDateDiscipline(TokenAccess token)
        {
            return _disciplineApi.GetFromDateDiscipline(token);
        }

        public CommandResult Update(IEnumerable<Discipline> disciplines, TokenAccess token)
        {
            return _disciplineApi.Update(disciplines, token);
        }
    }
}