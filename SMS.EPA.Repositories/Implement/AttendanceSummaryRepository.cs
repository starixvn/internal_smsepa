namespace SMS.EPA.Repositories.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.AttendanceSummary;
    using SMS.EPA.Model.Employee;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Repositories.Interfaces;

    public class AttendanceSummaryRepository : IAttendanceSummaryRepository
    {
        public IList<AttendanceSummaryModel> GetAttendanceSummary(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<AttendanceSummaryModel, EmployeeModel, Group, JobTitle, AttendanceSummaryModel>(
                    (att, emp, gr, job) =>
                    {
                        emp.JobTitle = job;
                        emp.Group = gr;
                        att.Employee = emp;
                        return att;
                    },
                    "EmployeeId, GroupId, JobTitleId",
                    criteria.GetSpParams()).ToList();
            }
        }

        public IList<AttendanceDetailModel> GetAttendanceDetail(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                IList<dynamic> violations = db.Query<dynamic>(criteria.GetSpParams()).ToList();

                IList<AttendanceDetailModel> violationDetails = MappingObjectForAttendanceDetail(violations);
                CalculateScore(violationDetails);

                return violationDetails;
            }
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        private static IList<AttendanceDetailModel> MappingObjectForAttendanceDetail(IList<dynamic> violationDetails)
        {
            return violationDetails.Select(violation =>
            {
                double totalHours = violation.TotalHours;
                int lateIn = violation.LateIn,
                    earlyOut = violation.EarlyOut;
                DateTime? checkIn = violation.CheckIn,
                    checkOut = violation.CheckOut;
                return new AttendanceDetailModel
                {
                    WorkDate = violation.WorkDate,
                    ShiftName = violation.ShiftName,
                    CheckIn = (checkIn.HasValue && checkIn.Value != default(DateTime)) ? checkIn.Value : (DateTime?)null,
                    CheckOut = (checkOut.HasValue && checkOut.Value != default(DateTime)) ? checkOut.Value : (DateTime?)null,
                    TotalHours = ConvertNumberToTime(totalHours * 60),
                    NumberOfMinutesLateIn = ConvertNumberToTime(lateIn),
                    NumberOfMinutesEarlyOut = ConvertNumberToTime(earlyOut),
                    Employee = new EmployeeModel
                    {
                        EmployeeId = violation.EmployeeId,
                        NickName = violation.NickName,
                        FullName = violation.FullName,
                        Code = violation.Code,
                        Category = violation.Category,
                        JoinedDate = violation.JoinedDate,
                        Group = new Group
                        {
                            GroupId = violation.GroupId,
                            GroupName = violation.GroupName,
                            GroupCode = violation.GroupCode
                        },
                        JobTitle = new JobTitle
                        {
                            JobTitleId = violation.JobTitleId,
                            JobTitleName = violation.JobTitleName
                        }
                    },
                    ChangeShift = GetChangeShift(violation),
                    MissingCheckInOut = GetMissingCheckInOut(violation),
                    LateIn = GetLateIn(violation),
                    EarlyOut = GetEarlyOut(violation),
                    Absent = GetAbsent(violation),
                    UnpaidLeave = GetUnpaidLeave(violation),
                    WorkingUnderTime = GetWorkingUnderTime(violation),
                    UnpaidLeaveAbsent = GetUnpaidLeaveAbsent(violation),
                    EmployeeComment = violation.EmployeeComment,
                    ApproverComment = violation.ApproverComment
                };
            }).ToList();
        }

        private static ViolationModel GetChangeShift(dynamic violation)
        {
            return new ViolationModel
            {
                ViolationId = violation.ChangeShiftId,
                Value = violation.ChangeShiftValue,
                Score = violation.ChangeShiftScore,
                HasIgnored = violation.ChangeShiftHasIgnored
            };
        }

        private static ViolationModel GetMissingCheckInOut(dynamic violation)
        {
            return new ViolationModel
            {
                ViolationId = violation.MissingCheckInOutId,
                Value = violation.MissingCheckInOutValue,
                Score = violation.MissingCheckInOutScore,
                HasIgnored = violation.MissingCheckInOutHasIgnored
            };
        }

        private static ViolationModel GetLateIn(dynamic violation)
        {
            return new ViolationModel
            {
                ViolationId = violation.LateInId,
                Value = violation.LateInValue,
                Score = violation.LateInScore,
                HasIgnored = violation.LateInHasIgnored
            };
        }

        private static ViolationModel GetEarlyOut(dynamic violation)
        {
            return new ViolationModel
            {
                ViolationId = violation.EarlyOutId,
                Value = violation.EarlyOutValue,
                Score = violation.EarlyOutScore,
                HasIgnored = violation.EarlyOutHasIgnored
            };
        }

        private static ViolationModel GetAbsent(dynamic violation)
        {
            return new ViolationModel
            {
                ViolationId = violation.AbsentId,
                Value = violation.AbsentValue,
                Score = violation.AbsentScore,
                HasIgnored = violation.AbsentHasIgnored
            };
        }

        private static ViolationModel GetUnpaidLeave(dynamic violation)
        {
            return new ViolationModel
            {
                ViolationId = violation.UnpaidLeaveId,
                Value = violation.UnpaidLeaveValue,
                Score = violation.UnpaidLeaveScore,
                HasIgnored = violation.UnpaidLeaveHasIgnored
            };
        }

        private static ViolationModel GetWorkingUnderTime(dynamic violation)
        {
            return new ViolationModel
            {
                ViolationId = violation.TotalHoursId,
                Value = violation.TotalHoursValue,
                Score = violation.TotalHoursScore,
                HasIgnored = violation.TotalHoursHasIgnored
            };
        }

        private static ViolationModel GetUnpaidLeaveAbsent(dynamic violation)
        {
            return new ViolationModel
            {
                ViolationId = violation.UnpaidLeaveAbsentId,
                Value = violation.UnpaidLeaveAbsentValue,
                Score = violation.UnpaidLeaveAbsentScore,
                HasIgnored = violation.UnpaidLeaveAbsentHasIgnored
            };
        }

        private static void CalculateScore(IList<AttendanceDetailModel> violationDetails)
        {
            foreach (AttendanceDetailModel violationDetail in violationDetails)
            {
                CalculateSubtractScore(violationDetail);
            }
        }

        private static void CalculateSubtractScore(AttendanceDetailModel violationDetail)
        {
            var violations = new List<ViolationModel>
            {
                violationDetail.MissingCheckInOut,
                violationDetail.LateIn,
                violationDetail.EarlyOut,
                violationDetail.WorkingUnderTime,
                violationDetail.Absent,
                violationDetail.UnpaidLeave,
                violationDetail.UnpaidLeaveAbsent,
                violationDetail.ChangeShift
            };

            foreach (ViolationModel violation in violations.Where(violation => violation != null))
            {
                violationDetail.SystemSubtractScore += violation.Value * violation.Score;
                violationDetail.ApproverSubtractScore += violation.HasIgnored.Equals(false) ? (violation.Value * violation.Score) : 0;
            }
        }

        private static string ConvertNumberToTime(double number)
        {
            string time = string.Empty;

            if (number > 0)
            {
                double numberMinutes = Math.Round(number);

                var timeSpan = new TimeSpan(0, (int)numberMinutes, 0);
                time = timeSpan.ToString(@"hh\:mm");
            }

            return time;
        }
    }
}