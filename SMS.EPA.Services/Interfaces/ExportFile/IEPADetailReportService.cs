namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using Model;

    public interface IEPADetailReportService
    {
        IList<string> GetColumnsName { get; }

        void Export(EPADetailModel epaDetail);

        void Render(EPADetailModel epaDetail);

        void WriteContentTable();
    }
}