namespace SMS.EPA.Services.Interfaces
{
    using Model;
    using System.Collections.Generic;

    public interface IEPADetailReportService
    {
        IList<string> GetColumnsName { get; }

        void Export(EPADetailModel epaDetail);

        void Render(EPADetailModel epaDetail);

        void WriteContentTable();
    }
}