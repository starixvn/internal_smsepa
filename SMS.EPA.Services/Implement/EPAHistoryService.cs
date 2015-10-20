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
        private IEPAHistoryRepository _epaHistoryRepository;

        public EPAHistoryService()
            : this(null)
        {
        }

        public EPAHistoryService(IEPAHistoryRepository epaHistoryRepository)
        {
            _epaHistoryRepository = epaHistoryRepository ?? new EPAHistoryRepository();
        }

        public IList<EPAHistoryModel> GetEPAHistory(GetEPAHistoryCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<EPAHistoryModel> epas = new List<EPAHistoryModel>();

            if (criteria.IsValid())
            {
                epas = _epaHistoryRepository.GetEPAHistory(criteria);
            }

            return epas;
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
