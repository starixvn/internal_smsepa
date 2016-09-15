namespace SMS.EPA.Model
{
    using System.Collections.Generic;

    public class EmployeeFlowSettingModel
    {
        public IEnumerable<EvaluatorInfoViewModel> DefaultFlowSettings { get; set; }

        public IEnumerable<EvaluatorInfoViewModel> PersonalFlowSettings { get; set; }
    }
}