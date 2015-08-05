// -----------------------------------------------------------------------
// <copyright file="FlowManagementController.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.17">Created</change>
// </history>

namespace SMS.EPA.Site.Controllers.API
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria.Employee;
    using SMS.EPA.Criteria.FlowManagement;
    using SMS.EPA.Model.Employee;
    using SMS.EPA.Model.FlowManagement;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class FlowManagementController : BaseController
    {
        private readonly IFlowManagementService _flowManagementService;
        private readonly IEmployeeService _employeeService;

        public FlowManagementController()
            : this(null, null)
        {
        }

        public FlowManagementController(
            IFlowManagementService flowManagementService,
            IEmployeeService employeeService)
        {
            _flowManagementService = flowManagementService ?? new FlowManagementService();
            _employeeService = employeeService ?? new EmployeeService();
        }

        [HttpGet]
        public IEnumerable<FlowTemplateModel> GetFlowTemplate(
            int? targetTypeId = null,
            bool? isActive = null)
        {
            var criteria = new GetFlowTemplateCriteria()
            {
                TargetTypeId = targetTypeId,
                IsActive = isActive
            };

            return _flowManagementService.GetFlowTemplate(criteria);
        }

        [HttpGet]
        public FlowTemplateModel GetFlowTemplateById(int flowTemplateId)
        {
            var criteria = new GetFlowTemplateByIdCriteria()
            {
                FlowTemplateId = flowTemplateId
            };

            return _flowManagementService.GetFlowTemplateById(criteria);
        }

        [HttpPost]
        public CommandResult CreateFlowTemplate(CreateFlowTemplateCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.CreatedBy = CurrentUser.EmployeeInfo.EmployeeId;
            }

            CommandResult commandResult = _flowManagementService.CreateFlowTemplate(criteria);

            return commandResult;
        }

        [HttpPut]
        public CommandResult UpdateFlowTemplate(UpdateFlowTemplateCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId;
            }

            CommandResult commandResult = _flowManagementService.UpdateFlowTemplate(criteria);

            return commandResult;
        }

        [HttpGet]
        public IEnumerable<FlowModel> GetDepartmentFlow(int departmentId)
        {
            IEnumerable<FlowModel> data = new List<FlowModel>();
            var criteria = new GetDepartmentFlowCriteria();

            if (departmentId > 0)
            {
                criteria.DepartmentId = departmentId;
                criteria.IsGroup = false;

                data = _flowManagementService.GetDepartmentFlow(criteria);
            }
            else if (departmentId < 0)
            {
                criteria.DepartmentId = Math.Abs(departmentId);
                criteria.IsGroup = true;

                data = _flowManagementService.GetDepartmentFlow(criteria);
            }

            return data;
        }

        [HttpGet]
        public IEnumerable<FlowModel> GetPersonalFlow(int employeeId)
        {
            IEnumerable<FlowModel> data = new List<FlowModel>();
            if (employeeId > 0)
            {
                var getEmployeeFlowCriteria = new GetPersonalFlowCriteria()
                {
                    EmployeeId = employeeId
                };

                data = _flowManagementService.GetPersonalFlow(getEmployeeFlowCriteria);
            }

            return data;
        }

        [HttpPost]
        public CommandResult CreateDepartmentFlow(InsertFlowModel model)
        {
            CommandResult result = new CommandResult();
            var criteria = new CreateDepartmentFlowCriteria();

            if (model.DepartmentId > 0)
            {
                criteria.DepartmentId = model.DepartmentId;
                criteria.IsGroup = false;
                criteria.FlowDetails = model.FlowDetails;
                criteria.ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId;

                result = _flowManagementService.CreateDepartmentFlow(criteria);
            }
            else if (model.DepartmentId < 0)
            {
                criteria.DepartmentId = Math.Abs(model.DepartmentId);
                criteria.IsGroup = true;
                criteria.FlowDetails = model.FlowDetails;
                criteria.ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId;

                result = _flowManagementService.CreateDepartmentFlow(criteria);
            }

            return result;
        }

        [HttpPost]
        public CommandResult CreatePersonalFlow(InsertFlowModel model)
        {
            CommandResult result = new CommandResult();

            var employeeFlowCriteria = new CreatePersonalFlowCriteria()
            {
                EmployeeId = model.Selfs.Employees[0].EmployeeId,
                FlowDetails = model.FlowDetails,
                ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId
            };

            result = _flowManagementService.CreatePersonalFlow(employeeFlowCriteria);

            return result;
        }

        [HttpDelete]
        public CommandResult DeleteDepartmentFlow(int departmentId)
        {
            CommandResult result = new CommandResult();
            var criteria = new DeleteDepartmentFlowCriteria();

            if (departmentId > 0)
            {
                criteria.DepartmentId = departmentId;
                criteria.IsGroup = false;
                criteria.ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId;

                result = _flowManagementService.DeleteDepartmentFlow(criteria);
            }
            else if (departmentId < 0)
            {
                criteria.DepartmentId = Math.Abs(departmentId);
                criteria.IsGroup = true;
                criteria.ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId;

                result = _flowManagementService.DeleteDepartmentFlow(criteria);
            }

            return result;
        }

        [HttpDelete]
        public CommandResult DeletePersonalFlow(int employeeId)
        {
            CommandResult result = new CommandResult();

            var criteria = new DeletePersonalFlowCriteria()
            {
                EmployeeId = employeeId,
                ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId
            };

            result = _flowManagementService.DeletePersonalFlow(criteria);

            return result;
        }

        [HttpGet]
        public EmployeeSettingFlowInDepartmentModel GetEmployeeSettingFlowInDepartment(int departmentId)
        {
            IEnumerable<EmployeeModel> employees = new List<EmployeeModel>();
            var result = new EmployeeSettingFlowInDepartmentModel();

            if (departmentId > 0)
            {
                var getEmployeeByDepartment = new GetEmployeeByDepartmentIdCriteria()
                {
                    DepartmentId = departmentId
                };

                employees = _employeeService.GetEmployeeByDepartmentId(getEmployeeByDepartment);
            }

            if (departmentId < 0)
            {
                var getEmployeeByGroup = new GetEmployeeByGroupIdCriteria()
                {
                    GroupId = Math.Abs(departmentId)
                };

                employees = _employeeService.GetEmployeeByGroupId(getEmployeeByGroup);
            }

            if (employees != null)
            {
                result.PersonalFlowSettingEmployees = employees.Where(x => x.IsExistsSetting == true);
                result.DefaultFlowSettingEmployees = employees.Where(x => x.IsExistsSetting == false);
            }

            return result;
        }
    }
}