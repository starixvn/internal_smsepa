namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using Criteria;
    using Criteria.EPAHistory;
    using Criteria.TargetType;
    using Model;
    using Model.Enitites;
    using SMS.EPA.Model.JobDescription;

    public interface ICommonService
    {
        IEnumerable<TargetTypeModel> GetTargetType(GetTargetTypeCriteria criteria);

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

        IEnumerable<PeriodModel> GetAllPeriodIncludeImport(GetAllPeriodIncludeImportCriteria criteria);

        IEnumerable<TargetTemplate> GetAllTargetTemplate(GetAllTargetTemplateCriteria criteria);

        IEnumerable<MenuGroupModel> GetMenusByEmployeeId(int employeeId);

        IEnumerable<JobTitle> GetAllJobTitle();

        IEnumerable<ScoreRatingTypeModel> GetScoreRatingType(GetScoreRatingTypeCriteria criteria);

        IEnumerable<JobDescriptionItem> GetJobDescriptionByEPAId(int epaId);
    }
}