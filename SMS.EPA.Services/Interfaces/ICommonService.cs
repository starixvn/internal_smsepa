namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using SMS.EPA.Criteria;
    using SMS.EPA.Criteria.Department;
    using SMS.EPA.Criteria.EPAHistory;
    using SMS.EPA.Criteria.Period;
    using SMS.EPA.Criteria.TargetType;
    using SMS.EPA.Model;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.Period;

    public interface ICommonService
    {
        IEnumerable<TargetType> GetTargetType(GetTargetTypeCriteria criteria);

        IEnumerable<KPIModel> GetKPI(GetKPICriteria criteria);

        IEnumerable<KPIGroupModel> GetKPIGroup(GetKPIGroupCriteria criteria);

        IEnumerable<Employee> GetAllEmployee(GetAllEmployeeCriteria criteria);

        IEnumerable<Employee> GetEmployeeByDepartmentId(int? departmentId);

        IEnumerable<Employee> GetEmployeeByGroupId(int groupId);

        Employee GetEmployeeById(int employeeId);

        IEnumerable<Group> GetAllGroup(GetAllGroupCriteria criteria);

        DepartmentInfoModel GetDepartment(GetDepartmentCriteria criteria);

        IEnumerable<PeriodModel> GetPeriod(GetPeriodCriteria criteria);

        IEnumerable<StandardGradeModel> GetStandardGrade();

        IList<PeriodModel> GetAllPeriodIncludeImport(GetAllPeriodIncludeImportCriteria criteria);

        IEnumerable<EmployeeOtherViolationDetailModel> GetOtherViolationDetailByEPAId(GetOtherViolationDetailByEPAIdCriteria criteria);

        IEnumerable<TargetTemplate> GetAllTargetTemplate(GetAllTargetTemplateCriteria criteria);

        IEnumerable<MenuGroupModel> GetMenusByEmployeeId(int employeeId);

        IEnumerable<JobTitle> GetAllJobTitle(GetAllJobTitleCriteria criteria);
    }
}