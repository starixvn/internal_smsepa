namespace SMS.EPA.ClientHrm.Services
{
    using Criteria;
    using Fanex.Data.Repository;
    using Model;
    using System;
    using System.Collections.Generic;
    using ThirdPartyAPI;

    public class DisciplineSummaryService : IDisciplineSummaryService
    {
        private readonly IDynamicRepository _dynamicRepository;
        private readonly IDisciplineSummaryAPI _attendanceApi;

        public DisciplineSummaryService(IDynamicRepository dynamicRepository, IDisciplineSummaryAPI attendanceApi)
        {
            _dynamicRepository = dynamicRepository;
            _attendanceApi = attendanceApi;
        }

        public IEnumerable<DisciplineSummary> GetDisciplineSummary(DateTime lastModifiedDate)
        {
            var criteria = new GetDisciplineSummaryCriteria
            {
                LastModifiedDate = lastModifiedDate
            };

            IEnumerable<DisciplineSummary> disciplines = _dynamicRepository.Fetch<DisciplineSummary>(criteria);

            return disciplines;
        }

        public DateTime GetLastModifiedDate(TokenAccess token)
        {
            return _attendanceApi.GetLastModifiedDate(token);
        }

        public CommandResult Update(IEnumerable<DisciplineSummary> disciplines, TokenAccess token)
        {
            return _attendanceApi.Update(disciplines, token);
        }
    }
}