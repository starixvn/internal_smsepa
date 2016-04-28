namespace SMS.EPA.ClientHRM.Services
{
    using Fanex.Data.Repository;
    using SMS.EPA.ClientHRM.Criteria;
    using SMS.EPA.ClientHRM.Model;
    using SMS.EPA.ClientHRM.ThirdPartyAPIs;
    using SMS.EPA.ServicesAPI.Model;
    using System;
    using System.Collections.Generic;

    public class AttendanceService : IAttendanceService
    {
        private readonly IDynamicRepository _dynamicRepository;
        private readonly IAttendanceAPI _attendanceAPI;

        public AttendanceService() : this(null, null)
        {
        }

        public AttendanceService(IDynamicRepository dynamicRepository, IAttendanceAPI attendanceAPI)
        {
            _dynamicRepository = dynamicRepository ?? new DynamicRepository();
            _attendanceAPI = attendanceAPI ?? new AttendanceAPI();
        }

        public IEnumerable<AttendanceViolation> GetViolation(DateTime fromDate, DateTime lastModifiedDate)
        {
            var criteria = new GetEmployeeAttendanceViolationCriteria
            {
                FromDate = fromDate,
                LastModifiedDate = lastModifiedDate
            };

            IEnumerable<AttendanceViolation> violations = _dynamicRepository.Fetch<AttendanceViolation>(criteria);

            return violations;
        }

        public DateTime GetMaxModifiedDate(TokenAccess token)
        {
            return _attendanceAPI.GetMaxModifiedDate(token);
        }

        public DateTime GetFromDateAttendanceViolation(TokenAccess token)
        {
            return _attendanceAPI.GetFromDateAttendanceViolation(token);
        }

        public CommandResult Update(IEnumerable<AttendanceViolation> violations, TokenAccess token)
        {
            return _attendanceAPI.Update(violations, token);
        }
    }
}
