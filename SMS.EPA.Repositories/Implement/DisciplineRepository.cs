namespace SMS.EPA.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Model.Discipline;
    using Model.Enitites;

    public class DisciplineRepository : IDisciplineRepository
    {
        public IEnumerable<DisciplineSummaryModel> GetDisciplineSummary(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<DisciplineSummaryModel, Employee, Group, JobTitle, DisciplineSummaryModel>(
                    (discipline, emp, gr, job) =>
                    {
                        emp.JobTitle = job;
                        emp.Group = gr;
                        discipline.Employee = emp;

                        return discipline;
                    },
                    "EmployeeId, GroupId, JobTitleId",
                    criteria.GetSpParams());
            }
        }

        public IEnumerable<DisciplineSummaryForOtherSystem> GetDisciplineSummaryForOtherSystem(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<DisciplineSummaryForOtherSystem, Employee, Group, JobTitle, DisciplineSummaryForOtherSystem>(
                    (discipline, emp, gr, job) =>
                    {
                        emp.JobTitle = job;
                        emp.Group = gr;
                        discipline.Employee = emp;

                        return discipline;
                    },
                    "EmployeeId, GroupId, JobTitleId",
                    criteria.GetSpParams()).ToList();
            }
        }
    }
}