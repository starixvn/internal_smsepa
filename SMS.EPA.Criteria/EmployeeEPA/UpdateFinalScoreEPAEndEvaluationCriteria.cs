namespace SMS.EPA.Criteria
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Model;

    public class UpdateFinalScoreEPAEndEvaluationCriteria : CriteriaBase
    {
        private const string TemplateXml = "<EPA EPAId='{0}' FinalScore='{1}' DisciplineScore='{2}' Grade='{3}'></EPA>";

        public int ModifiedBy { get; set; }

        [SpParam(Ignored = true)]
        public IEnumerable<EPAModel> EPAInfos { get; set; }

        public string EPAScores
        {
            get
            {
                var epaXmlBuilder = new StringBuilder();

                if (EPAInfos != null && EPAInfos.Any())
                {
                    epaXmlBuilder.Append(Constants.BeginRootTag);

                    foreach (EPAModel epaInfo in EPAInfos)
                    {
                        epaXmlBuilder.AppendFormat(TemplateXml, epaInfo.EPAId, epaInfo.FinalScore, epaInfo.DisciplineScore, epaInfo.Grade);
                    }

                    epaXmlBuilder.Append(Constants.EndRootTag);
                }

                return epaXmlBuilder.ToString();
            }
        }

        public override bool IsValid()
        {
            return ModifiedBy > 0 && !string.IsNullOrWhiteSpace(EPAScores);
        }

        public override string GetSettingKey()
        {
            return "EPA_UpdateFinalScoreEndEvaluation";
        }
    }
}