namespace SMS.EPA.Model
{
    using System.Collections.Generic;
    using System.Linq;

    public class ToDoListViewModel
    {
        public List<ToDoListModel> Probations { get; set; }

        public List<ToDoListModel> Promotions { get; set; }

        public List<ToDoListModel> Monthlies { get; set; }

        public List<ToDoListModel> SemiAnnuals { get; set; }

        public bool ProbationHasEvaluator
        {
            get
            {
                return Probations.Any(x => x.NumberOfEmployeesFinishedByEvaluator > 0 || x.NumberOfEmployeesUnfinishedByEvaluator > 0);
            }
        }

        public bool ProbationHasReviewer
        {
            get
            {
                return Probations.Any(x => x.NumberOfEmployeesFinishedByReviewer > 0 || x.NumberOfEmployeesUnfinishedByReviewer > 0);
            }
        }

        public bool ProbationHasManagement
        {
            get
            {
                return Promotions.Any(x => x.NumberOfEmployeesFinishedByManagement > 0 || x.NumberOfEmployeesUnfinishedByManagement > 0);
            }
        }

        public bool PromotionHasEvaluator
        {
            get
            {
                return Promotions.Any(x => x.NumberOfEmployeesFinishedByEvaluator > 0 || x.NumberOfEmployeesUnfinishedByEvaluator > 0);
            }
        }

        public bool PromotionHasReviewer
        {
            get
            {
                return Promotions.Any(x => x.NumberOfEmployeesFinishedByReviewer > 0 || x.NumberOfEmployeesUnfinishedByReviewer > 0);
            }
        }

        public bool PromotionHasManagement
        {
            get
            {
                return Probations.Any(x => x.NumberOfEmployeesFinishedByManagement > 0 || x.NumberOfEmployeesUnfinishedByManagement > 0);
            }
        }

        public bool MonthlyHasEvaluator
        {
            get
            {
                return Monthlies.Any(x => x.NumberOfEmployeesFinishedByEvaluator > 0 || x.NumberOfEmployeesUnfinishedByEvaluator > 0);
            }
        }

        public bool MonthlyHasReviewer
        {
            get
            {
                return Monthlies.Any(x => x.NumberOfEmployeesFinishedByReviewer > 0 || x.NumberOfEmployeesUnfinishedByReviewer > 0);
            }
        }

        public bool MonthlyHasManagement
        {
            get
            {
                return Monthlies.Any(x => x.NumberOfEmployeesFinishedByManagement > 0 || x.NumberOfEmployeesUnfinishedByManagement > 0);
            }
        }

        public bool SemiAnnualHasEvaluator
        {
            get
            {
                return SemiAnnuals.Any(x => x.NumberOfEmployeesFinishedByEvaluator > 0 || x.NumberOfEmployeesUnfinishedByEvaluator > 0);
            }
        }

        public bool SemiAnnualHasReviewer
        {
            get
            {
                return SemiAnnuals.Any(x => x.NumberOfEmployeesFinishedByReviewer > 0 || x.NumberOfEmployeesUnfinishedByReviewer > 0);
            }
        }

        public bool SemiAnnualHasManagement
        {
            get
            {
                return SemiAnnuals.Any(x => x.NumberOfEmployeesFinishedByManagement > 0 || x.NumberOfEmployeesUnfinishedByManagement > 0);
            }
        }
    }
}