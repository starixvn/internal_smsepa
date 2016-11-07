namespace SMS.EPA.Model
{
    using System.Collections.Generic;

    public class EPADetailModel
    {
        public EPADetailModel()
        {
            MemberEPAs = new List<EPAModel>();
            PhaseSelecteds = new List<EvaluationPhaseModel>();
        }

        public IEnumerable<EPAModel> MemberEPAs { get; set; }

        public IEnumerable<EvaluationPhaseModel> PhaseSelecteds { get; set; }
    }
}