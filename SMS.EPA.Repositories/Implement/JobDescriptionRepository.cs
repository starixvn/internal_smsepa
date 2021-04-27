namespace SMS.EPA.Repositories
{
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.JobDescription;

    public class JobDescriptionRepository : IJobDescriptionRepository
    {
        public async Task<JobDescriptionEmployeeSetting> GetEmployeeSettingByIdAsync(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = await db.QueryMultipleAsync(criteria.GetSpParams()))
                {
                    var employeeInfo = await multi.ReadFirstOrDefaultAsync<JobDescriptionEmployeeSetting>();
                    if (employeeInfo != null)
                    {
                        employeeInfo.GroupItems = await multi.ReadAsync<JobDescriptionGroupItem>();
                        var items = await multi.ReadAsync<JobDescriptionItem>();
                        foreach (var groupitem in employeeInfo.GroupItems)
                        {
                            groupitem.Items = items.Where(x => x.GroupItemId == groupitem.GroupItemId).ToList();
                        }
                    }

                    return employeeInfo;
                }
            }
        }
    }
}