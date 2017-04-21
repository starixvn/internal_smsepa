namespace SMS.EPA.ClientHrm.Services
{
    using System;
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using SMS.EPA.ClientHrm.Criteria;
    using SMS.EPA.ClientHrm.Model;
    using SMS.EPA.ClientHrm.ThirdPartyAPI;

    public class DisciplineService : IDisciplineService
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly IDisciplineAPI disciplineAPI;

        public DisciplineService(IDynamicRepository dynamicRepository, IDisciplineAPI attendanceAPI)
        {
            this.dynamicRepository = dynamicRepository;
            this.disciplineAPI = attendanceAPI;
        }

        public IEnumerable<Discipline> GetDiscipline(DateTime fromDate, DateTime lastModifiedDate)
        {
            var criteria = new GetDisciplineCriteria
            {
                FromDate = fromDate,
                LastModifiedDate = lastModifiedDate
            };

            IEnumerable<Discipline> violations = dynamicRepository.Fetch<Discipline>(criteria);

            return violations;
        }

        public DateTime GetLastModifiedDate(TokenAccess token)
        {
            return disciplineAPI.GetLastModifiedDate(token);
        }

        public DateTime GetFromDateDiscipline(TokenAccess token)
        {
            return disciplineAPI.GetFromDateDiscipline(token);
        }

        public CommandResult Update(IEnumerable<Discipline> disciplines, TokenAccess token)
        {
            return disciplineAPI.Update(disciplines, token);
        }
    }
}