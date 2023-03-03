namespace SMS.EPA.ClientHrm.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.UserControl.Client.Core.Models;
    using SMS.EPA.ClientHRM.Model.UnitHead;

    public interface IUnitHeadService
    {
        Task<IEnumerable<UnitHeadInfo>> GetUnitHeadInfoAsync();

        Task<IEnumerable<UnitHeadSchedule>> GetUnitHeadScheduleAsync();

        Task<CommandResult> UpdateUnitHeadInfoAsync(IEnumerable<UnitHeadInfo> infos);

        Task<CommandResult> UpdateUnitHeadScheduleAsync(IEnumerable<UnitHeadSchedule> schedules);
    }
}