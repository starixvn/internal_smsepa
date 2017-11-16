namespace SMS.EPA.Services.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Services;
    using Criteria.EPAHistory;
    using Interfaces;
    using Model.EPAHistory;
    using Repositories;

    public class EPAHistoryService : IEPAHistoryService
    {
        private readonly IEPAHistoryRepository epaHistoryRepository;
        private readonly ICryptographyService cryptographyService;

        public EPAHistoryService() : this(null, null)
        {
        }

        public EPAHistoryService(
            IEPAHistoryRepository epaHistoryRepository,
            ICryptographyService cryptographyService)
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
                if (!string.IsNullOrWhiteSpace(epa.EncodeFinalScore))
                {
                    epa.FinalScore = float.Parse(cryptographyService.Decrypt(epa.EncodeFinalScore));
                    var finalScoreDecimal = Math.Round((decimal)epa.FinalScore, 2);
                    epa.FinalScore = (float)finalScoreDecimal;
                }
                else
                {
                    epa.FinalScore = 0;
                }

                epa.Grade = !string.IsNullOrWhiteSpace(epa.EncodeGrade) ? cryptographyService.Decrypt(epa.EncodeGrade) : string.Empty;
            }
        }
    }
}