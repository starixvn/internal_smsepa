namespace SMS.EPA.ClientHrm.Services
{
    using System;
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using SMS.EPA.ClientHrm.Criteria;
    using SMS.EPA.ClientHrm.Model;
    using SMS.EPA.ClientHrm.ThirdPartyAPIs;

    public class DisciplineSummaryService : IDisciplineSummaryService
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly IDisciplineSummaryAPI attendanceAPI;

        public DisciplineSummaryService(IDynamicRepository dynamicRepository, IDisciplineSummaryAPI attendanceAPI)
        {
            this.dynamicRepository = dynamicRepository;
            this.attendanceAPI = attendanceAPI;
        }

        public IEnumerable<DisciplineSummary> GetDisciplineSummary(DateTime lastModifiedDate)
        {
            var criteria = new GetDisciplineSummaryCriteria
            {
                LastModifiedDate = lastModifiedDate
            };

            IEnumerable<DisciplineSummary> disciplines = dynamicRepository.Fetch<DisciplineSummary>(criteria);

            return disciplines;
        }

        public DateTime GetLastModifiedDate(TokenAccess token)
        {
            return attendanceAPI.GetLastModifiedDate(token);
        }

        public CommandResult Update(IEnumerable<DisciplineSummary> disciplines, TokenAccess token)
        {
            return attendanceAPI.Update(disciplines, token);
        }
    }
}