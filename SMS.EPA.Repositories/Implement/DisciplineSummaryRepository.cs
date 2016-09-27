namespace SMS.EPA.Repositories.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Model;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.Period;
    using SMS.EPA.Repositories.Interfaces;

    public class DisciplineSummaryRepository : IDisciplineSummaryRepository
    {
        public IEnumerable<DisciplineSummaryModel> GetDisciplineSummary(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<DisciplineSummaryModel, Employee, Group, JobTitle, DisciplineSummaryModel>(
                    (att, emp, gr, job) =>
                    {
                        emp.JobTitle = job;
                        emp.Group = gr;
                        att.Employee = emp;
                        return att;
                    },
                    "EmployeeId, GroupId, JobTitleId",
                    criteria.GetSpParams());
            }
        }

        public IEnumerable<DisciplineDetailModel> GetDisciplineDetail(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                IEnumerable<dynamic> violations = db.Query<dynamic>(criteria.GetSpParams());

                IEnumerable<DisciplineDetailModel> violationDetails = MappingObjectForAttendanceDetail(violations);
                CalculateScore(violationDetails);

                return violationDetails;
            }
        }

        public IEnumerable<DisciplineSummaryByPeriodModel> GetDisciplineSummaryByPeriod(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<DisciplineSummaryByPeriodModel, PeriodModel, Employee, Group, JobTitle, DisciplineSummaryByPeriodModel>(
                    (discipline, period, emp, gr, job) =>
                    {
                        emp.JobTitle = job;
                        emp.Group = gr;
                        discipline.Employee = emp;
                        discipline.Period = period;

                        return discipline;
                    },
                    "PeriodId, EmployeeId, GroupId, JobTitleId",
                    criteria.GetSpParams()).ToList();
            }
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        private IEnumerable<DisciplineDetailModel> MappingObjectForAttendanceDetail(IEnumerable<dynamic> violationDetails)
        {
            return violationDetails.Select(violation =>
            {
                double totalHours = violation.TotalHours;
                int lateIn = violation.LateIn,
                    earlyOut = violation.EarlyOut;
                DateTime? checkIn = violation.CheckIn,
                    checkOut = violation.CheckOut;
                return new DisciplineDetailModel
                {
                    WorkDate = violation.WorkDate,
                    ShiftName = violation.ShiftName,
                    CheckIn = (checkIn.HasValue && checkIn.Value != default(DateTime)) ? checkIn.Value : (DateTime?)null,
                    CheckOut = (checkOut.HasValue && checkOut.Value != default(DateTime)) ? checkOut.Value : (DateTime?)null,
                    TotalHours = ConvertNumberToTime(totalHours * 60),
                    NumberOfMinutesLateIn = ConvertNumberToTime(lateIn),
                    NumberOfMinutesEarlyOut = ConvertNumberToTime(earlyOut),
                    Employee = new Employee
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
            });
        }

        private ViolationModel GetChangeShift(dynamic violation)
        {
            return new ViolationModel
            {
                ViolationId = violation.ChangeShiftId,
                Value = violation.ChangeShiftValue,
                Score = violation.ChangeShiftScore,
                HasIgnored = violation.ChangeShiftHasIgnored
            };
        }

        private ViolationModel GetMissingCheckInOut(dynamic violation)
        {
            return new ViolationModel
            {
                ViolationId = violation.MissingCheckInOutId,
                Value = violation.MissingCheckInOutValue,
                Score = violation.MissingCheckInOutScore,
                HasIgnored = violation.MissingCheckInOutHasIgnored
            };
        }

        private ViolationModel GetLateIn(dynamic violation)
        {
            return new ViolationModel
            {
                ViolationId = violation.LateInId,
                Value = violation.LateInValue,
                Score = violation.LateInScore,
                HasIgnored = violation.LateInHasIgnored
            };
        }

        private ViolationModel GetEarlyOut(dynamic violation)
        {
            return new ViolationModel
            {
                ViolationId = violation.EarlyOutId,
                Value = violation.EarlyOutValue,
                Score = violation.EarlyOutScore,
                HasIgnored = violation.EarlyOutHasIgnored
            };
        }

        private ViolationModel GetAbsent(dynamic violation)
        {
            return new ViolationModel
            {
                ViolationId = violation.AbsentId,
                Value = violation.AbsentValue,
                Score = violation.AbsentScore,
                HasIgnored = violation.AbsentHasIgnored
            };
        }

        private ViolationModel GetUnpaidLeave(dynamic violation)
        {
            return new ViolationModel
            {
                ViolationId = violation.UnpaidLeaveId,
                Value = violation.UnpaidLeaveValue,
                Score = violation.UnpaidLeaveScore,
                HasIgnored = violation.UnpaidLeaveHasIgnored
            };
        }

        private ViolationModel GetWorkingUnderTime(dynamic violation)
        {
            return new ViolationModel
            {
                ViolationId = violation.TotalHoursId,
                Value = violation.TotalHoursValue,
                Score = violation.TotalHoursScore,
                HasIgnored = violation.TotalHoursHasIgnored
            };
        }

        private ViolationModel GetUnpaidLeaveAbsent(dynamic violation)
        {
            return new ViolationModel
            {
                ViolationId = violation.UnpaidLeaveAbsentId,
                Value = violation.UnpaidLeaveAbsentValue,
                Score = violation.UnpaidLeaveAbsentScore,
                HasIgnored = violation.UnpaidLeaveAbsentHasIgnored
            };
        }

        private void CalculateScore(IEnumerable<DisciplineDetailModel> violationDetails)
        {
            foreach (DisciplineDetailModel violationDetail in violationDetails)
            {
                CalculateSubtractScore(violationDetail);
            }
        }

        private void CalculateSubtractScore(DisciplineDetailModel violationDetail)
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

        private string ConvertNumberToTime(double number)
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