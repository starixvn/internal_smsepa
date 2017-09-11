namespace SMS.EPA.Services.Implement
{
    using Core.Services;
    using Criteria;
    using Interfaces;
    using Model;
    using Model.Discipline;
    using Repositories;
    using System.Collections.Generic;
    using System.Linq;

    public class DisciplineService : BaseService, IDisciplineService
    {
        private readonly IDisciplineSummaryRepository disciplineSummaryRepository;

        public DisciplineService() : this(null)
        {
        }

        public DisciplineService(IDisciplineSummaryRepository disciplineSummaryRepository)
        {
            this.disciplineSummaryRepository = disciplineSummaryRepository ?? new DisciplineSummaryRepository();
        }

        public IEnumerable<DisciplineSummaryModel> GetDisciplineSummary(GetDisciplineSummaryCriteria criteria)
        {
            IEnumerable<DisciplineSummaryModel> employeeDisciplines = new List<DisciplineSummaryModel>();

            if (criteria != null && criteria.IsValid())
            {
                employeeDisciplines = disciplineSummaryRepository.GetDisciplineSummary(criteria);
                employeeDisciplines = employeeDisciplines.OrderByDescending(x => x.Permission);
            }

            return employeeDisciplines;
        }

        public IEnumerable<OtherDisciplineModel> GetOtherViolations(GetOtherViolationsByEmployeeIdCriteria criteria)
        {
            IEnumerable<OtherDisciplineModel> violations = new List<OtherDisciplineModel>();

            if (criteria != null && criteria.IsValid())
            {
                violations = GetData<OtherDisciplineModel>(criteria);
            }

            return violations;
        }

        public IEnumerable<DisciplineTypeModel> GetDisciplineType(GetDisciplineTypeCriteria criteria)
        {
            IEnumerable<DisciplineTypeModel> violations = new List<DisciplineTypeModel>();

            if (criteria != null && criteria.IsValid())
            {
                violations = GetData<DisciplineTypeModel>(criteria);
            }

            return violations;
        }

        public IEnumerable<DisciplineDetailModel> GetDisciplineDetail(GetDisciplineDetailCriteria criteria)
        {
            IEnumerable<DisciplineDetailModel> employeeDisciplines = new List<DisciplineDetailModel>();

            if (criteria != null && criteria.IsValid())
            {
                employeeDisciplines = disciplineSummaryRepository.GetDisciplineDetail(criteria);
                employeeDisciplines = employeeDisciplines.OrderBy(x => x.Employee.Group.GroupId)
                    .ThenBy(x => x.Employee.Code)
                    .ThenByDescending(x => x.WorkDate);
            }

            return employeeDisciplines;
        }

        public IEnumerable<DisciplineSummaryByPeriodModel> GetDisciplineSummaryByPeriod(GetDisciplineSummaryByPeriodCriteria criteria)
        {
            IEnumerable<DisciplineSummaryByPeriodModel> employeeDisciplines = new List<DisciplineSummaryByPeriodModel>();

            if (criteria != null && criteria.IsValid())
            {
                employeeDisciplines = disciplineSummaryRepository.GetDisciplineSummaryByPeriod(criteria);
            }

            return employeeDisciplines;
        }
    }
}