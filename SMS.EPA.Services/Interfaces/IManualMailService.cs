namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using SMS.EPA.Model;

    public interface IManualMailService
    {
        void SendStartMail(PeriodModel period, IEnumerable<PeriodFlowModel> employeeSettings);

        void SendNotifyMail(PeriodModel period, IEnumerable<PeriodFlowModel> employeeSettings);
    }
}