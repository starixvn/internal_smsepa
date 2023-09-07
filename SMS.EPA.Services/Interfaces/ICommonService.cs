namespace SMS.EPA.Services
{
    using System;
    using System.Collections.Generic;
    using HRM.UserControl.Client.Core.Models.Role;
    using SMS.EPA.Criteria;
    using SMS.EPA.Criteria.EPAHistory;
    using SMS.EPA.Criteria.TargetType;
    using SMS.EPA.Model;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.JobDescription;

    public interface ICommonService
    {
        IEnumerable<TargetTypeModel> GetTargetType(GetTargetTypeCriteria criteria);

        IEnumerable<KPIModel> GetKPI(GetKPICriteria criteria);

        IEnumerable<KPIGroupModel> GetKPIGroup(GetKPIGroupCriteria criteria);

        Employee GetEmployeeById(int employeeId, IList<Role> roles);

        IEnumerable<Group> GetAllGroup(GetAllGroupCriteria criteria);

        IEnumerable<PeriodModel> GetPeriod(GetPeriodCriteria criteria);

        IEnumerable<StandardGradeModel> GetStandardGrade();

        IEnumerable<PeriodModel> GetAllPeriodIncludeImport(GetAllPeriodIncludeImportCriteria criteria);

        IEnumerable<TargetTemplate> GetAllTargetTemplate(GetAllTargetTemplateCriteria criteria);

        IEnumerable<JobTitle> GetAllJobTitle();

        IEnumerable<ScoreRatingTypeModel> GetScoreRatingType(GetScoreRatingTypeCriteria criteria);

        IEnumerable<JobDescriptionGroupItem> GetJobDescriptionByEPAId(int epaId);

        string CalculateLengthOfService(DateTime? fromDate, DateTime? toDate, DateTime? militaryFromDate, DateTime? militaryToDate);
    }
}