namespace SMS.EPA.ServicesAPI.Controllers
{
    using SMS.EPA.Core.Model;
    using SMS.EPA.ServicesAPI.Criteria.EmployeeDiscipline;
    using SMS.EPA.ServicesAPI.Model;
    using SMS.EPA.ServicesAPI.Services;
    using System;
    using System.Collections.Generic;
    using System.Web.Http;

    public class EmployeeDisciplineController : ApiController
    {
         private readonly IServiceAPI _service;

        public EmployeeDisciplineController() : this(null) { }

        public EmployeeDisciplineController(IServiceAPI service)
        {
            _service = service ?? new ServiceAPI();
        }

         public IHttpActionResult GetMaxModifiedDate()
         {
             var criteria = new GetMaxModifiedDateForAttendanceViolationCriteria();

             DateTime fromDate = _service.GetMaxModifiedDate(criteria);

             return Ok(fromDate);
         }

         public IHttpActionResult GetFromDateAttendanceViolation()
         {
             var criteria = new GetFromDateForAttendanceViolationCriteria();

             DateTime fromDate = _service.GetMaxModifiedDate(criteria);

             return Ok(fromDate);
         }

         public IHttpActionResult Update([FromBody]EmployeeDiscipline[] employees)
         {
             var criteria = new UpdateAttendanceViolationCriteria()
             {
                 AttendanceViolations = employees
             };

             CommandResult result = _service.Update(criteria);

             return Ok(result);
         }
    }
}
