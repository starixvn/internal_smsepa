// ----------------------------------------------------------------------
// <copyright file="AttendanceSummaryRepository.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="9/09/2015">Created.</change>
// </history>

namespace SMS.EPA.Repositories.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.AttendanceSummary;
    using SMS.EPA.Model.Employee;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Repositories.Interfaces;

    public class AttendanceSummaryRepository : IAttendanceSummaryRepository
    {
        public IEnumerable<AttendanceSummaryModel> GetAttendanceSummary(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<AttendanceSummaryModel, Group, AttendanceSummaryModel>(
                                        (att, gr) =>
                                        {
                                            att.Group = gr;
                                            return att;
                                        },
                                        "GroupId",
                                        criteria.GetSpParams());
            }
        }

        public RawAttendanceDetailModel GetAttendanceDetail(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    IEnumerable<EmployeeModel> employees = multi.Read<EmployeeModel, Group, EmployeeModel>(
                        (emp, gr) =>
                        {
                            emp.Group = gr;
                            return emp;
                        },
                        "GroupId");

                    IEnumerable<AttendanceDetailModel> firstViolations = 
                        multi.Read<AttendanceDetailModel, ViolationModel, ViolationModel, ViolationModel, ViolationModel, ViolationModel, AttendanceDetailModel>(
                        (attD, chIn, chO, tot, laI, earO) =>
                        {
                            attD.CheckIn = chIn;
                            attD.CheckOut = chO;
                            attD.TotalHours = tot;
                            attD.LateIn = laI;
                            attD.EarlyOut = earO;
                            return attD;
                        },
                        "CheckInId, CheckOutId, TotalHoursId, LateInId, EarlyOutId");

                    IEnumerable<AttendanceDetailModel> lastViolations = 
                        multi.Read<AttendanceDetailModel, ViolationModel, ViolationModel, ViolationModel, ViolationModel, AttendanceDetailModel>(
                        (attD, ab, up, ch, upa) =>
                        {
                            attD.Absent = ab;
                            attD.UnpaidLeave = up;
                            attD.UnpaidLeaveAbsent = upa;
                            attD.ChangeShift = ch;
                            return attD;
                        },
                        "AbsentId, UnpaidLeaveId, ChangeShiftId, UnpaidLeaveAbsentId");

                    IEnumerable<AttendanceDetailModel> commentViolations = multi.Read<AttendanceDetailModel>();

                    IEnumerable<AttendanceDetailModel> violations = new List<AttendanceDetailModel>();

                    if (firstViolations != null && firstViolations.Count() > 0)
                    {
                        violations = violations.Concat(firstViolations);
                    }

                    if (lastViolations != null && lastViolations.Count() > 0)
                    {
                        violations = violations.Concat(lastViolations);
                    }

                    var rawAttendanceDetail = new RawAttendanceDetailModel();

                    if (employees != null && employees.Count() > 0)
                    {
                        rawAttendanceDetail.Employees = employees;
                        rawAttendanceDetail.Violations = violations.ToList();
                        rawAttendanceDetail.CommentViolations = commentViolations;
                    }

                    return rawAttendanceDetail;
                  }
             }
       }
    }
}
