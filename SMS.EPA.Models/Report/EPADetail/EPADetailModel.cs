namespace SMS.EPA.Model
{
    using System.Collections.Generic;
    using Report;

    public class EPADetailModel : EPAReportConditionalFilename
    {
        public EPADetailModel()
        {
            EpaInfos = new List<EPAModel>();
            PhaseSelecteds = new List<EvaluationPhaseModel>();
        }

        public IEnumerable<EPAModel> EpaInfos { get; set; }

        public IEnumerable<EvaluationPhaseModel> PhaseSelecteds { get; set; }
    }
}