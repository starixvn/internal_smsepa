namespace SMS.EPA.Services
{
    using System.Linq;
    using System.Threading.Tasks;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.JobDescription;
    using SMS.EPA.Model.JobDescription;

    public class JobDescriptionService : IJobDescriptionService
    {
        private readonly IBaseService baseService;

        public JobDescriptionService() : this(null)
        {
        }

        public JobDescriptionService(IBaseService baseService)
        {
            this.baseService = baseService ?? new BaseService();
        }

        public async Task<JobDescriptionGroupSetting> GetGroupSettingByIdAsync(int groupId, int jobLevelId)
        {
            var criteria = new GetGroupSettingJobDescriptionByIdCriteria { GroupId = groupId, JobLevelId = jobLevelId };
            var tuple = await baseService.FetchMultipleAsync<JobDescriptionGroupSetting, JobDescriptionGroupItem, JobDescriptionItem>(criteria);
            var groupInfo = tuple.Item1.FirstOrDefault();
            if (groupInfo != null)
            {
                groupInfo.GroupItems = tuple.Item2;
                foreach (var groupitem in groupInfo.GroupItems)
                {
                    groupitem.Items = tuple.Item3.Where(x => x.GroupItemId == groupitem.GroupItemId).ToList();
                }

                groupInfo.GroupItems = groupInfo.GroupItems.OrderBy(x => x.DisplayOrder).ToList();
            }

            return groupInfo;
        }

        public async Task<JobDescriptionEmployeeSetting> GetEmployeeSettingByIdAsync(int employeeId)
        {
            var criteria = new GetEmployeeSettingJobDescriptionByIdCriteria { EmployeeId = employeeId };
            var tuple = await baseService.FetchMultipleAsync<JobDescriptionEmployeeSetting, JobDescriptionGroupItem, JobDescriptionItem>(criteria);
            var employeeInfo = tuple.Item1.FirstOrDefault();
            if (employeeInfo != null)
            {
                employeeInfo.GroupItems = tuple.Item2;
                foreach (var groupitem in employeeInfo.GroupItems)
                {
                    groupitem.Items = tuple.Item3.Where(x => x.GroupItemId == groupitem.GroupItemId).ToList();
                }

                employeeInfo.GroupItems = employeeInfo.GroupItems.OrderBy(x => x.DisplayOrder).ToList();
            }

            return employeeInfo;
        }
    }
}