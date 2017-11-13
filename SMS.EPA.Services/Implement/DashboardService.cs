namespace SMS.EPA.Services.Implement
{
    using Core.Services;
    using Criteria;
    using Interfaces;
    using Model;

    public class DashboardService : BaseService, IDashboardService
    {
        public ToDoListViewModel GetToDoList(int employeeId)
        {
            var criteria = new GetToDoListCriteria
            {
                EmployeeId = employeeId
            };
            var result = new ToDoListViewModel();

            if (criteria.IsValid())
            {
                result.ToDoList = GetData<ToDoListModel>(criteria);
            }

            return result;
        }
    }
}