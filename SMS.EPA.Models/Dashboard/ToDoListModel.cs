namespace SMS.EPA.Model
{
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Model.Period;

    public class ToDoListModel
    {
        public PeriodModel Period { get; set; }

        public int NumberOfEmployeesFinishedByEvaluator { get; set; }

        public int NumberOfEmployeesUnfinishedByEvaluator { get; set; }

        public int NumberOfEmployeesFinishedByReviewer { get; set; }

        public int NumberOfEmployeesUnfinishedByReviewer { get; set; }

        public int NumberOfEmployeesFinishedByManagement { get; set; }

        public int NumberOfEmployeesUnfinishedByManagement { get; set; }

        public bool IsMonthly
        {
            get
            {
                return Period.TargetType.TargetTypeId == (int)TargetTypes.Monthly;
            }
        }
    }
}