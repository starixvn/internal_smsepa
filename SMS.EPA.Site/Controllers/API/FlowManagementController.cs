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
        public IEnumerable<FlowModel> GetFlow(int departmentId, int flowType)
        {
            IEnumerable<FlowModel> data = new List<FlowModel>();
            if (flowType > 0)
            {
                var departmentFlowCriteria = new GetDepartmentFlowCriteria() { DepartmentId = departmentId };
                data = _flowManagementService.GetFlowDepartment(departmentFlowCriteria);
            }
            else if (flowType < 0)
            {
                var groupFlowCriteria = new GetGroupFlowCriteria() { GroupId = Math.Abs(departmentId) };
                data = _flowManagementService.GetFlowGroup(groupFlowCriteria);
            }

            return data;
        }

        [HttpGet]
        public IEnumerable<FlowModel> GetFlowEmployee(int employeeId)
        {
            IEnumerable<FlowModel> data = new List<FlowModel>();
            if (employeeId > 0)
            {
                var getEmployeeFlowCriteria = new GetEmployeeFlowCriteria()
                {
                    EmployeeId = employeeId
                };

                data = _flowManagementService.GetFlowEmployee(getEmployeeFlowCriteria);
            }

            return data;
        }

        [HttpPost]
        public CommandResult CreateFlow(InsertFlowModel model)
        {
            CommandResult result = new CommandResult();

            if (model.FlowType > 0)
            {
                var departmentFlowCriteria = new CreateDepartmentFlowCriteria()
                {
                    DepartmentId = model.DepartmentId,
                    ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId,
                    FlowDetails = model.FlowDetails
                };

                result = _flowManagementService.CreateDepartmentFlow(departmentFlowCriteria);
            }
            else if (model.FlowType < 0)
            {
                var groupFlowCriteria = new CreateGroupFlowCriteria()
                {
                    GroupId = Math.Abs(model.DepartmentId),
                    ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId,
                    FlowDetails = model.FlowDetails
                };

                result = _flowManagementService.CreateGroupFlow(groupFlowCriteria);
            }

            return result;
        }

        [HttpPost]
        public CommandResult CreateEmployeeFlow(InsertFlowModel model)
        {
            CommandResult result = new CommandResult();

            var employeeFlowCriteria = new CreateEmployeeFlowCriteria()
            {
                EmployeeId = model.Selfs.Employees[0].EmployeeId,
                FlowDetails = model.FlowDetails,
                ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId
            };

            result = _flowManagementService.CreateEmployeeFlow(employeeFlowCriteria);

            return result;
        }

        [HttpDelete]
        public CommandResult DeleteFlow(int departmentId, int flowType)
        {
            CommandResult result = new CommandResult();

            if (flowType > 0)
            {
                var deleteDepartmentFlowCriteria = new DeleteDepartmentFlowCriteria()
                {
                    DepartmentId = departmentId,
                    ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId
                };

                result = _flowManagementService.DeleteDepartmentFlow(deleteDepartmentFlowCriteria);
            }
            else if (flowType < 0)
            {
                var deleteGroupFlowCriteria = new DeleteGroupFlowCriteria()
                {
                    GroupId = departmentId,
                    ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId
                };

                result = _flowManagementService.DeleteGroupFlow(deleteGroupFlowCriteria);
            }

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