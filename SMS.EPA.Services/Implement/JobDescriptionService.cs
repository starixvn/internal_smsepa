namespace SMS.EPA.Services
{
    using System.Linq;
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

        public JobDescriptionGroupSetting GetGroupSettingById(int groupId, int jobLevelId)
        {
            var criteria = new GetGroupSettingJobDescriptionByIdCriteria { GroupId = groupId, JobLevelId = jobLevelId };
            (var groupInfos, var groupItems, var items) = baseService.FetchMultiple<JobDescriptionGroupSetting, JobDescriptionGroupItem, JobDescriptionItem>(criteria);
            var groupInfo = groupInfos.FirstOrDefault();
            if (groupInfo != null)
            {
                groupInfo.GroupItems = groupItems;
                foreach (var groupitem in groupInfo.GroupItems)
                {
                    groupitem.Items = items.Where(x => x.GroupItemId == groupitem.GroupItemId).ToList();
                }

                groupInfo.GroupItems = groupInfo.GroupItems.OrderBy(x => x.DisplayOrder).ToList();
            }

            return groupInfo;
        }

        public JobDescriptionEmployeeSetting GetEmployeeSettingById(int employeeId)
        {
            var criteria = new GetEmployeeSettingJobDescriptionByIdCriteria { EmployeeId = employeeId };
            (var employeeInfos, var groupItems, var items) = baseService.FetchMultiple<JobDescriptionEmployeeSetting, JobDescriptionGroupItem, JobDescriptionItem>(criteria);
            var employeeInfo = employeeInfos.FirstOrDefault();
            if (employeeInfo != null)
            {
                employeeInfo.GroupItems = groupItems;
                foreach (var groupitem in employeeInfo.GroupItems)
                {
                    groupitem.Items = items.Where(x => x.GroupItemId == groupitem.GroupItemId).ToList();
                }

                employeeInfo.GroupItems = employeeInfo.GroupItems.OrderBy(x => x.DisplayOrder).ToList();
            }

            return employeeInfo;
        }
    }
}