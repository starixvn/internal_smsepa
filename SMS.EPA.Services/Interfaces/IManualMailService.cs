namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using Model;

    public interface IManualMailService
    {
        void SendStartMail(PeriodModel period, IEnumerable<PeriodFlowModel> employeeSettings);

        void SendNotifyMail(PeriodModel period, IEnumerable<PeriodFlowModel> employeeSettings);
    }
}