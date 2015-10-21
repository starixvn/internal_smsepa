// ----------------------------------------------------------------------
// <copyright file="EPAHistoryService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="10/19/2015">Created.</change>
// </history>

namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Common.Helper;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.EPAHistory;
    using SMS.EPA.Model.EPAHistory;
    using SMS.EPA.Model.Period;
    using SMS.EPA.Repositories.Implement;
    using SMS.EPA.Repositories.Interfaces;
    using SMS.EPA.Services.Interfaces;

    public class EPAHistoryService : BaseService, IEPAHistoryService
    {
        private readonly IEPAHistoryRepository _epaHistoryRepository;
        private readonly IEmployeeEPAService _employeeEPAService;

        public EPAHistoryService()
            : this(null, null)
        {
        }

        public EPAHistoryService(IEPAHistoryRepository epaHistoryRepository, IEmployeeEPAService employeeEPAService)
        {
            _epaHistoryRepository = epaHistoryRepository ?? new EPAHistoryRepository();
            _employeeEPAService = employeeEPAService ?? new EmployeeEPAService();
        }

        public IList<EPAHistoryModel> GetEPAHistory(GetEPAHistoryCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<EPAHistoryModel> epas = new List<EPAHistoryModel>();

            if (criteria.IsValid())
            {
                epas = _epaHistoryRepository.GetEPAHistory(criteria);

                if (epas != null && epas.Any())
                {
                    DecodeFinalScoreAndGrade(epas);
                }
            }

            return epas;
        }

        private void DecodeFinalScoreAndGrade(IList<EPAHistoryModel> epas)
        {
            foreach (EPAHistoryModel epa in epas)
            {
                epa.FinalScore = !string.IsNullOrWhiteSpace(epa.EncodeFinalScore) ? float.Parse(EncodeService.Decode(epa.EncodeFinalScore)) : 0;
                epa.Grade = !string.IsNullOrWhiteSpace(epa.EncodeGrade) ? EncodeService.Decode(epa.EncodeGrade) : string.Empty;

                if (string.IsNullOrWhiteSpace(epa.Grade) && epa.FinalScore >= 0)
                {
                    epa.Grade = _employeeEPAService.ConvertScoreToGrade(epa.FinalScore);
                }
            }
        }

        public IList<PeriodModel> GetAllPeriodIncludeImport(GetAllPeriodIncludeImportCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<PeriodModel> periods = new List<PeriodModel>();

            if (criteria.IsValid())
            {
                periods = GetData<PeriodModel>(criteria).ToList();
            }

            return periods;
        }
    }
}
