namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.EPAHistory;
    using SMS.EPA.Model;
    using SMS.EPA.Model.EPAHistory;
    using SMS.EPA.Repositories;
    using SMS.EPA.Services.Interfaces;

    public class EPAHistoryService : BaseService, IEPAHistoryService
    {
        private readonly IEPAHistoryRepository epaHistoryRepository;
        private readonly ICryptographyService cryptographyService;

        public EPAHistoryService()
            : this(null, null)
        {
        }

        public EPAHistoryService(IEPAHistoryRepository epaHistoryRepository, ICryptographyService cryptographyService)
        {
            this.epaHistoryRepository = epaHistoryRepository ?? new EPAHistoryRepository();
            this.cryptographyService = cryptographyService ?? new CryptographyService();
        }

        public IList<EPAHistoryModel> GetEPAHistory(GetEPAHistoryCriteria criteria)
        {
            IList<EPAHistoryModel> epas = new List<EPAHistoryModel>();

            if (criteria != null && criteria.IsValid())
            {
                epas = epaHistoryRepository.GetEPAHistory(criteria);

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
                epas = epaHistoryRepository.GetMonthlyEPAHistory(criteria);

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
                epa.FinalScore = !string.IsNullOrWhiteSpace(epa.EncodeFinalScore) ? float.Parse(cryptographyService.Decrypt(epa.EncodeFinalScore)) : 0;
                epa.Grade = !string.IsNullOrWhiteSpace(epa.EncodeGrade) ? cryptographyService.Decrypt(epa.EncodeGrade) : string.Empty;
            }
        }

        public IList<PeriodModel> GetAllPeriodIncludeImport(GetAllPeriodIncludeImportCriteria criteria)
        {
            var periods = GetData<PeriodModel>(criteria).ToList();
            return periods;
        }
    }
}