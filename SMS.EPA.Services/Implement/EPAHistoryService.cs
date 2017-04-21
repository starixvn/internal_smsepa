namespace SMS.EPA.Services.Implement
{
    using Core.Services;
    using Criteria.EPAHistory;
    using Interfaces;
    using Model;
    using Model.EPAHistory;
    using Repositories;
    using System.Collections.Generic;
    using System.Linq;

    public class EPAHistoryService : BaseService, IEPAHistoryService
    {
        private readonly IEPAHistoryRepository _epaHistoryRepository;
        private readonly ICryptographyService _cryptographyService;

        public EPAHistoryService()
            : this(null, null)
        {
        }

        public EPAHistoryService(IEPAHistoryRepository epaHistoryRepository, ICryptographyService cryptographyService)
        {
            _epaHistoryRepository = epaHistoryRepository ?? new EPAHistoryRepository();
            _cryptographyService = cryptographyService ?? new CryptographyService();
        }

        public IList<EPAHistoryModel> GetEPAHistory(GetEPAHistoryCriteria criteria)
        {
            IList<EPAHistoryModel> epas = new List<EPAHistoryModel>();

            if (criteria != null && criteria.IsValid())
            {
                epas = _epaHistoryRepository.GetEPAHistory(criteria);

                if (epas != null && epas.Any())
                {
                    DecodeFinalScoreAndGrade(epas);
                }
            }

            return epas;
        }

        public IList<EPAHistoryModel> GetMonthlyEPAHistory(GetMonthlyEPAHistoryCriteria criteria)
        {
            IList<EPAHistoryModel> epas = new List<EPAHistoryModel>();

            if (criteria != null && criteria.IsValid())
            {
                epas = _epaHistoryRepository.GetMonthlyEPAHistory(criteria);

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
                epa.FinalScore = !string.IsNullOrWhiteSpace(epa.EncodeFinalScore) ? float.Parse(_cryptographyService.Decrypt(epa.EncodeFinalScore)) : 0;
                epa.Grade = !string.IsNullOrWhiteSpace(epa.EncodeGrade) ? _cryptographyService.Decrypt(epa.EncodeGrade) : string.Empty;
            }
        }

        public IList<PeriodModel> GetAllPeriodIncludeImport(GetAllPeriodIncludeImportCriteria criteria)
        {
            var periods = GetData<PeriodModel>(criteria).ToList();
            return periods;
        }
    }
}