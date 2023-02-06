namespace SMS.EPA.Model.Dashboard
{
    public class JobLevelStandardDashboard
    {
        public int PeriodId { get; set; }

        public string Name { get; set; }

        public int FinishedByEvaluator { get; set; }
        public int UnfinishedByEvaluator { get; set; }

        public int FinishedByManagement { get; set; }
        public int UnfinishedByManagement { get; set; }
    }
}