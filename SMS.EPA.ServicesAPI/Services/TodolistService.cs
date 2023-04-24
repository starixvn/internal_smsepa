namespace SMS.EPA.ServicesAPI.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Services;
    using SMS.EPA.ServicesAPI.Criteria.Todolist;
    using SMS.EPA.ServicesAPI.Models.Todolist;

    public class TodolistService : ITodolistService
    {
        private readonly IBaseService baseService;

        public TodolistService(IBaseService baseService = null)
        {
            this.baseService = baseService ?? new BaseService();
        }

        public async Task<IList<TodolistItem>> GetEPAByUserIdAsync(int userId)
        {
            var criteria = new GetTodolistEPAByUserIdCriteria
            {
                UserId = userId
            };
            var epas = await baseService.GetDataAsync<TodolistEPA>(criteria);
            var items = new List<TodolistItem>();
            foreach (var epa in epas)
            {
                var item = new TodolistItem();
                switch (epa.ProcessId)
                {
                    case (byte)ProcessType.Self:
                        item.Name = string.Format("<div><strong>{0}</strong> EPA is ready for your self-evaluation.</div>" +
                            "<div>You are given a period from <strong>{1}</strong> until <strong>{2}</strong> to complete this self-evaluation.</div>",
                            epa.Name,
                            FormatStartDate(epa.StartDate),
                            FormatEndDate(epa.EndDate));
                        item.Actions = GetAction(epa);

                        items.Add(item);
                        break;

                    case (byte)ProcessType.Evaluator:
                        item.Name = string.Format("<div><strong>{0}</strong> EPA is ready for your assessment on your subordinate(s) performance.</div>" +
                            "<div>You are given a period from <strong>{1}</strong> until <strong>{2}</strong> to complete this evaluation.</div>",
                            epa.Name,
                            FormatStartDate(epa.StartDate),
                            FormatEndDate(epa.EndDate));
                        item.Actions = GetAction(epa);

                        items.Add(item);
                        break;

                    case (byte)ProcessType.Reviewer:
                    case (byte)ProcessType.Management:
                        item.Name = string.Format("<div><strong>{0}</strong> EPA is ready for your review the EPA of your subordinate(s).</div>" +
                            "<div>You are given a period from <strong>{1}</strong> until <strong>{2}</strong> to complete this evaluation.</div>",
                            epa.Name,
                            FormatStartDate(epa.StartDate),
                            FormatEndDate(epa.EndDate));
                        item.Actions = GetAction(epa);

                        items.Add(item);
                        break;

                    default:
                        // nothing
                        break;
                }
            }

            return items;
        }

        private static IList<TodolistAction> GetAction(TodolistEPA epa)
        {
            var action = new TodolistAction
            {
                Name = "View",
                Method = "Get",
                IsRedirect = true
            };
            switch (epa.ProcessId)
            {
                case (byte)ProcessType.Self:
                    action.Url = "/SelfEvaluation/Index";
                    break;

                case (byte)ProcessType.Evaluator:
                    action.Url = string.Format("/Evaluator/Index?periodId={0}&isMonthly={1}", epa.PeriodId, epa.TargetTypeId == (byte)TargetType.Monthly);
                    break;

                case (byte)ProcessType.Reviewer:
                    action.Url = string.Format("/Reviewer/Index?periodId={0}&isMonthly={1}", epa.PeriodId, epa.TargetTypeId == (byte)TargetType.Monthly);
                    break;

                case (byte)ProcessType.Management:
                    action.Url = string.Format("/Management/Index?periodId={0}&isMonthly={1}", epa.PeriodId, epa.TargetTypeId == (byte)TargetType.Monthly);
                    break;

                default:
                    // nothing
                    break;
            }

            return new List<TodolistAction> { action };
        }

        private static string FormatStartDate(DateTime startDate)
        {
            var result = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0, 0, startDate.Kind);
            return result.ToString(Constants.FormatDateTimeEmail);
        }

        private static string FormatEndDate(DateTime endDate)
        {
            var result = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 0, 0, endDate.Kind);
            return result.ToString(Constants.FormatDateTimeEmail);
        }
    }
}