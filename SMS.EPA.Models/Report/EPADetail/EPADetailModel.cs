namespace SMS.EPA.Model
{
    using System.Collections.Generic;

    public class EPADetailModel
    {
        public IEnumerable<EPAModel> MemberEPAs { get; set; }

        public IEnumerable<EvaluationPhaseModel> PhaseSelecteds { get; set; }

        public int TotalItem { get; set; }
    }
}