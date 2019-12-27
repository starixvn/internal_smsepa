namespace SMS.EPA.ClientHrm.Services
{
    using Core.Model;
    using Criteria;
    using EPA.Model.API;
    using Fanex.Data.Repository;
    using Model;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
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

        public async Task<DateTime> GetLastModifiedDateAsync(TokenAccess token)
        {
            return await _attendanceApi.GetLastModifiedDateAsync(token);
        }

        public async Task<CommandResult> UpdateAsync(IEnumerable<DisciplineSummary> disciplines, TokenAccess token)
        {
            return await _attendanceApi.UpdateAsync(disciplines, token);
        }
    }
}