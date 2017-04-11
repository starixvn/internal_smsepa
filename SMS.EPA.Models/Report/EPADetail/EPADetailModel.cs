namespace SMS.EPA.Model
{
    using System.Collections.Generic;

    public class EPADetailModel
    {
        public EPADetailModel()
        {
            EpaInfos = new List<EPAModel>();
            PhaseSelecteds = new List<EvaluationPhaseModel>();
        }

        public IEnumerable<EPAModel> EpaInfos { get; set; }

        public IEnumerable<EvaluationPhaseModel> PhaseSelecteds { get; set; }

        public string EpaYear { get; set; }

        public string EpaEvaluationPeriod { get; set; }
    }
}