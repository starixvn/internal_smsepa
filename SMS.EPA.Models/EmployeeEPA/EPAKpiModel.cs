namespace SMS.EPA.Model.EmployeeEPA
{
    using System.Collections.Generic;

    public class EPAKPIModel
    {
        public EPAKPIModel()
        {
            EvaluationByEvaluators = new List<EvaluationModel>();
        }

        public int EPAId { get; set; }

        public int KPIGroupId { get; set; }

        public int KPIId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool HasComment { get; set; }

        public bool IsRequiredComment { get; set; }

        public bool IsRequiredEvaluate { get; set; }

        public double Weight { get; set; }

        public EvaluationModel CurrentEvaluation { get; set; }

        public EvaluationModel EvaluationBySelf { get; set; }

        public IList<EvaluationModel> EvaluationByEvaluators { get; set; }
    }
}