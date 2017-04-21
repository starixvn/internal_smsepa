namespace SMS.EPA.Services.Implement
{
    using Core.Enumerations;
    using Core.Services;
    using Criteria;
    using Model;
    using Repositories;
    using System.Collections.Generic;
    using System.Linq;

    using Interfaces;

    public class DashboardService : BaseService, IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;

        public DashboardService()
            : this(null)
        {
        }

        public DashboardService(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository ?? new DashboardRepository();
        }

        public ToDoListViewModel GetToDoList(int employeeId)
        {
            var criteria = new GetToDoListCriteria
            {
                EmployeeId = employeeId
            };
            var result = new ToDoListViewModel();

            if (criteria.IsValid())
            {
                IEnumerable<ToDoListModel> todoList = _dashboardRepository.GetToDoList(criteria);
                result.ToDoList = todoList.Where(x => x.Period.TargetType.TargetTypeId == (int)TargetType.Probation)
                    .Concat(todoList.Where(x => x.Period.TargetType.TargetTypeId == (int)TargetType.SemiAnnual))
                    .Concat(todoList.Where(x => x.Period.TargetType.TargetTypeId == (int)TargetType.Promotion))
                    .Concat(todoList.Where(x => x.Period.TargetType.TargetTypeId == (int)TargetType.Monthly)).ToList();
            }

            return result;
        }
    }
}