namespace SMS.EPA.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Model;
    using SMS.EPA.Model.Enitites;

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