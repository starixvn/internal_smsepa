namespace SMS.EPA.Model
{
    using System.Collections.Generic;
    using System.Linq;

    public class EPAToDoListViewModel
    {
        public IEnumerable<EPAToDoList> ToDoList { get; set; } = new List<EPAToDoList>();

        public bool HasEvaluator => ToDoList.Any(x => x.UnfinishedByEvaluator > 0 || x.FinishedByEvaluator > 0);

        public bool HasReviewer => ToDoList.Any(x => x.UnfinishedByReviewer > 0 || x.FinishedByReviewer > 0);

        public bool HasManagement => ToDoList.Any(x => x.UnfinishedByManagement > 0 || x.FinishedByManagement > 0);
    }
}