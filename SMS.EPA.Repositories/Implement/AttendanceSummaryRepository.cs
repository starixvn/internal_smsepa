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
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.AttendanceSummary;
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
                    IEnumerable<AttendanceDetailModel> firstViolations = multi.Read<AttendanceDetailModel, Group, ViolationModel, ViolationModel, ViolationModel, ViolationModel, ViolationModel, AttendanceDetailModel>(
                        (attD, gr, chIn, chO, tot, laI, earO) =>
                        {
                            attD.Group = gr;
                            attD.CheckIn = chIn;
                            attD.CheckOut = chO;
                            attD.TotalHours = tot;
                            attD.LateIn = laI;
                            attD.EarlyOut = earO;
                            return attD;
                        }
                        , "GroupId, CheckInId, CheckOutId, TotalHoursId, LateInId, EarlyOutId");

                    IEnumerable<AttendanceDetailModel> lastViolations = multi.Read<AttendanceDetailModel, Group, ViolationModel, ViolationModel, ViolationModel, ViolationModel, AttendanceDetailModel>(
                        (attD, gr, ab, up, ch, upa) =>
                        {
                            attD.Group = gr;
                            attD.Absent = ab;
                            attD.UnpaidLeave = up;
                            attD.UnpaidLeaveAbsent = upa;
                            attD.ChangeShift = ch;
                            return attD;
                        }
                        , "GroupId, AbsentId, UnpaidLeaveId, ChangeShiftId, UnpaidLeaveAbsent");

                    var rawAttendanceDetail = new RawAttendanceDetailModel
                    {
                        FirstViolations = firstViolations,
                        LastViolations = lastViolations
                    };

                    return rawAttendanceDetail;
                  }
             }
       }
    }
}
