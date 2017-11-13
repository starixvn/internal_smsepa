namespace SMS.EPA.Model
{
    using Core.Enumerations;

    public class ToDoListModel
    {
        public int PeriodId { get; set; }

        public string Name { get; set; }

        public int TargetTypeId { get; set; }

        public int FinishedByEvaluator { get; set; }

        public int UnfinishedByEvaluator { get; set; }

        public int FinishedByReviewer { get; set; }

        public int UnfinishedByReviewer { get; set; }

        public int FinishedByManagement { get; set; }

        public int UnfinishedByManagement { get; set; }

        public bool IsMonthly => TargetTypeId == (int)TargetType.Monthly;
    }
}