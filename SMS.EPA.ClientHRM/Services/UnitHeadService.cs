namespace SMS.EPA.ClientHrm.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GO.UserControl.Client.Core.Models;
    using GO.UserControl.Client.Core.Services;
    using SMS.EPA.ClientHRM.Criteria.UnitHead;
    using SMS.EPA.ClientHRM.Model.UnitHead;
    using SMS.EPA.Core.Services;

    public class UnitHeadService : BaseGOHttpClient, IUnitHeadService
    {
        private readonly IBaseService baseService;

        public UnitHeadService(
            IBaseService baseService,
            ICryptoService cryptoService) : base(cryptoService)
        {
            this.baseService = baseService;
        }

        public async Task<IEnumerable<UnitHeadInfo>> GetUnitHeadInfoAsync()
        {
            var criteria = new GetUnitHeadInfoCriteria();
            return await baseService.GetDataAsync<UnitHeadInfo>(criteria);
        }

        public async Task<IEnumerable<UnitHeadSchedule>> GetUnitHeadScheduleAsync()
        {
            var criteria = new GetUnitHeadScheduleCriteria();
            return await baseService.GetDataAsync<UnitHeadSchedule>(criteria);
        }

        public async Task<CommandResult> UpdateUnitHeadInfoAsync(IEnumerable<UnitHeadInfo> infos)
        {
            await GetTokenAsync();
            if (!_jwttoken.IsExpired)
            {
                return await PostAsync<IEnumerable<UnitHeadInfo>, CommandResult>
                    ("api/UnitHead/UpdateInfo", infos, _jwttoken.Token);
            }

            return new CommandResult();
        }

        public async Task<CommandResult> UpdateUnitHeadScheduleAsync(IEnumerable<UnitHeadSchedule> schedules)
        {
            await GetTokenAsync();
            if (!_jwttoken.IsExpired)
            {
                return await PostAsync<IEnumerable<UnitHeadSchedule>, CommandResult>
                    ("api/UnitHead/UpdateSchedule", schedules, _jwttoken.Token);
            }

            return new CommandResult();
        }
    }
}