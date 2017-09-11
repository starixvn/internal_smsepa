namespace SMS.EPA.ClientHrm.Services
{
    using Criteria;
    using EPA.Model.API;
    using Fanex.Data.Repository;
    using Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

        public void Update(IEnumerable<Discipline> disciplines, TokenAccess token)
        {
            var disciplineList = disciplines.ToList();
            List<Discipline> disciplineTemps;
            var skipNumbers = 0;
            var takeNumbers = 100;
            while (skipNumbers <= disciplineList.Count)
            {
                disciplineTemps = disciplineList.Skip(skipNumbers).Take(takeNumbers).ToList();
                _disciplineApi.Update(disciplineTemps, token);
                skipNumbers += takeNumbers;
            }
        }
    }
}