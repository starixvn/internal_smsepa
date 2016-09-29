namespace SMS.EPA.Model
{
    using System.Collections.Generic;
    using System.Linq;

    public class ToDoListViewModel
    {
        public List<ToDoListModel> ToDoList { get; set; }

        public bool HasEvaluator
        {
            get
            {
                return ToDoList.Any(x => x.NumberOfEmployeesFinishedByEvaluator > 0 || x.NumberOfEmployeesUnfinishedByEvaluator > 0);
            }
        }

        public bool HasReviewer
        {
            get
            {
                return ToDoList.Any(x => x.NumberOfEmployeesFinishedByReviewer > 0 || x.NumberOfEmployeesUnfinishedByReviewer > 0);
            }
        }

        public bool HasManagement
        {
            get
            {
                return ToDoList.Any(x => x.NumberOfEmployeesFinishedByManagement > 0 || x.NumberOfEmployeesUnfinishedByManagement > 0);
            }
        }
    }
}