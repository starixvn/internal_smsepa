namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Repositories.Implement;
    using SMS.EPA.Repositories.Interfaces;
    using SMS.EPA.Services.Interfaces;

    public class DashboardService : BaseService, IDashboardService
    {
        private readonly IDashboardRepository dashboardRepository;

        public DashboardService()
            : this(null)
        {
        }

        public DashboardService(IDashboardRepository dashboardRepository)
        {
            this.dashboardRepository = dashboardRepository ?? new DashboardRepository();
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
                IEnumerable<ToDoListModel> todoList = dashboardRepository.GetToDoList(criteria);
                result.Probations = todoList.Where(x => x.Period.TargetType.TargetTypeId == (int)TargetTypes.Probation).ToList();
                result.Promotions = todoList.Where(x => x.Period.TargetType.TargetTypeId == (int)TargetTypes.Promotion).ToList();
                result.Monthlies = todoList.Where(x => x.Period.TargetType.TargetTypeId == (int)TargetTypes.Monthly).ToList();
                result.SemiAnnuals = todoList.Where(x => x.Period.TargetType.TargetTypeId == (int)TargetTypes.SemiAnnual).ToList();
            }

            return result;
        }
    }
}