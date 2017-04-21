namespace SMS.EPA.Services.Implement
{
    using Core.Services;
    using Criteria;
    using Interfaces;
    using Model;
    using Repositories;
    using System.Collections.Generic;
    using System.Linq;

    public class DisciplineService : BaseService, IDisciplineService
    {
        private readonly IDisciplineSummaryRepository _disciplineSummaryRepository;

        public DisciplineService()
            : this(null)
        {
        }

        public DisciplineService(IDisciplineSummaryRepository disciplineSummaryRepository)
        {
            _disciplineSummaryRepository = disciplineSummaryRepository ?? new DisciplineSummaryRepository();
        }

        public IEnumerable<DisciplineSummaryModel> GetDisciplineSummary(GetDisciplineSummaryCriteria criteria)
        {
            IEnumerable<DisciplineSummaryModel> employeeDisciplines = new List<DisciplineSummaryModel>();

            if (criteria != null && criteria.IsValid())
            {
                employeeDisciplines = _disciplineSummaryRepository.GetDisciplineSummary(criteria);
                employeeDisciplines = employeeDisciplines.OrderByDescending(x => x.Permission);
            }

            return employeeDisciplines;
        }

        public IEnumerable<OtherViolationModel> GetOtherViolations(GetOtherViolationsByEmployeeIdCriteria criteria)
        {
            IEnumerable<OtherViolationModel> violations = new List<OtherViolationModel>();

            if (criteria != null && criteria.IsValid())
            {
                violations = GetData<OtherViolationModel>(criteria);
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
                employeeDisciplines = _disciplineSummaryRepository.GetDisciplineDetail(criteria);
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
                employeeDisciplines = _disciplineSummaryRepository.GetDisciplineSummaryByPeriod(criteria);
            }

            return employeeDisciplines;
        }
    }
}