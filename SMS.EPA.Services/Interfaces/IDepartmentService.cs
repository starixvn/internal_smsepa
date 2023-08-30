namespace SMS.EPA.Services
{
    using System.Collections.Generic;
    using Model.Enitites;

    public interface IDepartmentService
    {
        IList<MultiSelectItem> GetOrgChart();
    }
}