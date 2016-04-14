namespace SMS.EPA.ServicesAPI.Controllers
{
    using SMS.EPA.Core.Model;
    using SMS.EPA.ServicesAPI.Criteria.Employee;
    using SMS.EPA.ServicesAPI.Model;
    using SMS.EPA.ServicesAPI.Services;
    using System;
    using System.Collections.Generic;
    using System.Web.Http;

    public class EmployeeController : ApiController
    {
        private readonly IServiceAPI _employeeService;

        public EmployeeController(IServiceAPI employeeService)
        {
            _employeeService = employeeService ?? new ServiceAPI();
        }

        public IHttpActionResult GetMaxModifiedDate()
        {
            var criteria = new GetMaxModifiedDateForEmployeeCriteria();

            DateTime fromDate = _employeeService.GetMaxModifiedDate(criteria);

            return Ok(fromDate);
        }

        public IHttpActionResult Update(IList<Employee> employees)
        {
            var criteria = new UpdateEmployeeCriteria()
            {
                Employees = employees
            };

            CommandResult result = _employeeService.Update(criteria);

            return Ok(result);
        }
    }
}
