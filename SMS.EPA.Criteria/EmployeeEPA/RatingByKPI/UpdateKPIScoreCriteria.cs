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

    public class UpdateKPIScoreCriteria : CriteriaBase
    {
        private const string EPAXmlTemplate = "<EPA EPAId='{0}' EmployeeId='{1}' KPIGroupId='{2}' TotalScore='{3}'>{4}</EPA>";
        private const string EPADetailXmlTemplate = "<EPADetail KPIId ='{0}' Rate='{1}' Score='{2}' IsMaxScore='{3}' Comment='{4}' />";
        private readonly ICryptographyService _cryptographyService = new CryptographyService();

        public int EvaluatorId { get; set; }

        public string KPIDetails => ConvertToXml(EpaInfos);

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

        private static bool IsKPINotNullAndRatePossitive(TemporaryKPIModel kpi)
        {
            return !string.IsNullOrEmpty(kpi.Score) || kpi.Rate > 0;
        }

        private string ConvertToXml(IEnumerable<RatingByKPIModel> epaInfos)
        {
            var epaXml = new StringBuilder(Constants.BeginRootTag);

            foreach (RatingByKPIModel epaInfo in epaInfos)
            {
                foreach (TemporaryKPIModel kpi in epaInfo.KPIGroup.KPIs)
                {
                    var epaDetailXml = string.IsNullOrEmpty(kpi.AdjustScore)
                                                 && (kpi.AdjustRate <= 0)
                                                 && IsKPINotNullAndRatePossitive(kpi)
                        ? GenerateEpaDetailXml(kpi, kpi.Score, kpi.Rate)
                        : GenerateEpaDetailXml(kpi, kpi.AdjustScore, kpi.AdjustRate);

                    string totalScore = _cryptographyService.Encrypt(epaInfo.TotalScore);
                    epaXml.AppendFormat(
                        EPAXmlTemplate,
                        epaInfo.EPAId,
                        epaInfo.Employee.EmployeeId,
                        epaInfo.KPIGroup.KPIGroupId,
                        totalScore,
                        epaDetailXml);
                }
            }

            epaXml.Append(Constants.EndRootTag);

            return epaXml.ToString();
        }

        private StringBuilder GenerateEpaDetailXml(TemporaryKPIModel kpi, string score, float rate)
        {
            string kpiRate = rate > 0 ? rate.ToString(CultureInfo.InvariantCulture) : string.Empty;
            string cipherScore = _cryptographyService.Encrypt(score + "_" + kpi.KPIId);
            string cipherRate = _cryptographyService.Encrypt(kpiRate);

            var epaDetailXml = new StringBuilder();
            epaDetailXml.AppendFormat(EPADetailXmlTemplate, kpi.KPIId, cipherRate, cipherScore, kpi.IsMaxScore, HttpUtility.HtmlEncode(kpi.Comment));

            return epaDetailXml;
        }
    }
}