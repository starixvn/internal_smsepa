namespace SMS.EPA.Repositories
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Model;
    using System.Collections.Generic;

    public class DashboardRepository : IDashboardRepository
    {
        public IEnumerable<ToDoListModel> GetToDoList(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                IEnumerable<ToDoListModel> departments = db.Query<ToDoListModel, PeriodModel, TargetTypeModel, ToDoListModel>(
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