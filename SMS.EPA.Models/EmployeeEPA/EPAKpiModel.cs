namespace SMS.EPA.Model
{
    using Core.Constants;
    using Core.Services;
    using System.Collections.Generic;
    using System.Globalization;

    public class EPAKpiModel
    {
        public EPAKpiModel()
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

        public string WeightToDisplay
        {
            get
            {
                var result = string.Empty;

                if (Weight > 0)
                {
                    IUtilityService utilityService = new UtilityService();
                    result = utilityService.SplitNumber(Weight, Constants.NumberOfDecimalDefault).ToString(CultureInfo.InvariantCulture);
                }

                return result;
            }
        }

        public EvaluationModel CurrentEvaluation { get; set; }

        public EvaluationModel EvaluationBySelf { get; set; }

        public IList<EvaluationModel> EvaluationByEvaluators { get; set; }
    }
}