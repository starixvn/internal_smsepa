namespace SMS.EPA.Criteria
{
    using System.Collections.Generic;
    using System.Text;
    using System.Web;
    using Fanex.Data.Repository;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Model;

    public class UpdateKPIScoreCriteria : CriteriaBase
    {
        private const string EPAXmlTemplate = "<EPA EPAId='{0}' EmployeeId='{1}' KPIGroupId='{2}' TotalScore='{3}'>{4}</EPA>";
        private const string EPADetailXmlTemplate = "<EPADetail KPIId ='{0}' Rate='{1}' Score='{2}' IsMaxScore='{3}' Comment='{4}' />";

        public int EvaluatorId { get; set; }

        public string KPIDetails
        {
            get
            {
                return ConvertToXml(EpaInfos);
            }
        }

        [SpParam(Ignored = true)]
        public IEnumerable<RatingByKPIModel> EpaInfos { get; set; }

        public override string GetSettingKey()
        {
            return "EPA_RatingByKPI_UpdateKPIScore";
        }

        public override bool IsValid()
        {
            return EvaluatorId > 0;
        }

        private string Encrypt(string plainText)
        {
            ICryptographyService cryptographyService = new CryptographyService();

            return cryptographyService.Encrypt(plainText);
        }

        private string ConvertToXml(IEnumerable<RatingByKPIModel> epaInfos)
        {
            var epaXml = new StringBuilder(Constants.BeginRootTag);

            foreach (RatingByKPIModel epaInfo in epaInfos)
            {
                var epaDetailXml = new StringBuilder();

                foreach (TemporaryKPIModel kpi in epaInfo.KPIGroup.KPIs)
                {
                    if (!string.IsNullOrEmpty(kpi.AdjustScore) || kpi.AdjustRate > 0)
                    {
                        epaDetailXml = GenerateEpaDetailXml(kpi, kpi.AdjustScore, kpi.AdjustRate);
                    }
                    else if (!string.IsNullOrEmpty(kpi.Score) || kpi.Rate > 0)
                    {
                        epaDetailXml = GenerateEpaDetailXml(kpi, kpi.Score, kpi.Rate);
                    }

                    string totalScore = Encrypt(epaInfo.TotalScore);
                    epaXml.AppendFormat(EPAXmlTemplate, epaInfo.EPAId, epaInfo.Employee.EmployeeId, epaInfo.KPIGroup.KPIGroupId, totalScore, epaDetailXml);
                }
            }

            epaXml.Append(Constants.EndRootTag);

            return epaXml.ToString();
        }

        private StringBuilder GenerateEpaDetailXml(TemporaryKPIModel kpi, string score, float rate)
        {
            string kpiRate = rate > 0 ? rate.ToString() : string.Empty;
            string cipherScore = Encrypt(score + "_" + kpi.KPIId);
            string cipherRate = Encrypt(kpiRate);

            var epaDetailXml = new StringBuilder();
            epaDetailXml.AppendFormat(EPADetailXmlTemplate, kpi.KPIId, cipherRate, cipherScore, kpi.IsMaxScore, HttpUtility.HtmlEncode(kpi.Comment));

            return epaDetailXml;
        }
    }
}