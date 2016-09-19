namespace SMS.EPA.Repositories.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Model;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Repositories.Interfaces;

    public class DisciplineSummaryRepository : IDisciplineSummaryRepository
    {
        public IEnumerable<DisciplineSummaryModel> GetDisciplineSummaryByPeriod(ICriteria criteria)
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
                    criteria.GetSpParams()).ToList();
            }
        }
    }
}