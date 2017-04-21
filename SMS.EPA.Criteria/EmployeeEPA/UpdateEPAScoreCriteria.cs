namespace SMS.EPA.Criteria
{
    using Core.Constants;
    using Core.Services;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Model;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using System.Web;

    public class UpdateEPAScoreCriteria : CriteriaBase
    {
        private const string KPIScoreXmlTemplate = "<KPI KPIId='{0}' KPIGroupId='{1}' Score='{2}' Rate='{3}' IsMaxScore='{4}' Comment='{5}'></KPI>";
        private readonly ICryptographyService _cryptographyService;

        public UpdateEPAScoreCriteria()
        {
            _cryptographyService = new CryptographyService();
        }

        public int EPAId { get; set; }

        public int ProcessId { get; set; }

        public int EmployeeId { get; set; }

        public int EvaluatorId { get; set; }

        public string Comment => EPAProcess.Comment;

        public string Score
        {
            get
            {
                string totalScore = EPAProcess.TotalScore;
                return _cryptographyService.Encrypt(totalScore);
            }
        }

        public string KPIDetails
        {
            get
            {
                var kpiDetailXml = new StringBuilder(Constants.BeginRootTag);

                foreach (EPAKpiGroupModel kpiGroup in KPIGroups)
                {
                    foreach (EPAKpiModel kpi in kpiGroup.KPIs)
                    {
                        string score = kpi.CurrentEvaluation.Score;
                        string rate = kpi.CurrentEvaluation.Rate > 0 ? kpi.CurrentEvaluation.Rate.ToString(CultureInfo.InvariantCulture) : string.Empty;
                        string cipherScore = _cryptographyService.Encrypt(score + "_" + kpi.KPIId);
                        string cipherRate = _cryptographyService.Encrypt(rate);
                        bool isMaxScore = kpi.CurrentEvaluation.IsMaxScore;

                        kpiDetailXml.AppendFormat(
                            KPIScoreXmlTemplate,
                            kpi.KPIId,
                            kpiGroup.KPIGroupId,
                            cipherScore,
                            cipherRate,
                            isMaxScore,
                            HttpUtility.HtmlEncode(kpi.CurrentEvaluation.Comment));
                    }
                }

                kpiDetailXml.Append(Constants.EndRootTag);

                return kpiDetailXml.ToString();
            }
        }

        public int ModifiedBy { get; set; }

        [SpParam(Ignored = true)]
        public EPAProcessModel EPAProcess { get; set; }

        [SpParam(Ignored = true)]
        public IList<EPAKpiGroupModel> KPIGroups { get; set; }

        public override bool IsValid()
        {
            return EPAId > 0 && EmployeeId > 0 && EvaluatorId > 0 && ProcessId > 0;
        }

        public override string GetSettingKey()
        {
            return "EPA_UpdateScore";
        }
    }
}