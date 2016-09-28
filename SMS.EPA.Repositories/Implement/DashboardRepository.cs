namespace SMS.EPA.Repositories.Implement
{
    using System.Collections.Generic;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Model;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.Period;
    using SMS.EPA.Repositories.Interfaces;

    public class DashboardRepository : IDashboardRepository
    {
        public IEnumerable<ToDoListModel> GetToDoList(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                IEnumerable<ToDoListModel> departments = db.Query<ToDoListModel, PeriodModel, TargetType, ToDoListModel>(
                   (todo, period, ttype) =>
                   {
                       period.TargetType = ttype;
                       todo.Period = period;

                       return todo;
                   },
                   "PeriodId, TargetTypeId",
                   criteria.GetSpParams());

                return departments;
            }
        }
    }
}