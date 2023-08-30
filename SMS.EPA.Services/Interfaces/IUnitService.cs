namespace SMS.EPA.Services
{
    using System.Collections.Generic;
    using SMS.EPA.Model.Enitites;

    public interface IUnitService
    {
        IList<Unit> Get(bool? isActive = true);

        IList<MultiSelectItem> GetOrgChart();
    }
}