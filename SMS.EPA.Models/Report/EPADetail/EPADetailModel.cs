namespace SMS.EPA.Model.Report.EPADetail
{
    using System.Collections.Generic;
    using SMS.EPA.Model;

    public class EPADetailModel
    {
        public IEnumerable<EPAModel> MemberEPAs { get; set; }

        public IEnumerable<EvaluationPhaseModel> PhaseSelecteds { get; set; }

        public int TotalItem { get; set; }
    }
}