namespace SMS.EPA.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Model;
    using Model.Discipline;
    using Model.Enitites;

    public class DisciplineSummaryRepository : IDisciplineSummaryRepository
    {
        public IEnumerable<DisciplineSummaryModel> GetDisciplineSummary(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<DisciplineSummaryModel, Employee, Group, JobTitle, DisciplineSummaryModel>(
                    (att, emp, gr, job) =>
                    {
                        emp.JobTitle = job;
                        emp.Group = gr;
                        att.Employee = emp;
                        return att;
                    },
                    "EmployeeId, GroupId, JobTitleId",
                    criteria.GetSpParams());
            }
        }

        public IEnumerable<DisciplineSummaryByPeriodModel> GetDisciplineSummaryByPeriod(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<DisciplineSummaryByPeriodModel, PeriodModel, Employee, Group, JobTitle, DisciplineSummaryByPeriodModel>(
                    (discipline, period, emp, gr, job) =>
                    {
                        emp.JobTitle = job;
                        emp.Group = gr;
                        discipline.Employee = emp;
                        discipline.Period = period;

                        return discipline;
                    },
                    "PeriodId, EmployeeId, GroupId, JobTitleId",
                    criteria.GetSpParams()).ToList();
            }
        }
    }
}