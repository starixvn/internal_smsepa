namespace SMS.EPA.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Enumerations;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Model;
    using Model.Enitites;

    public class FlowSettingRepository : IFlowSettingRepository
    {
        public IEnumerable<FlowModel> GetFlowSettingByFlowTemplateId(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var epaFlow = db.Query<FlowModel, FlowTemplateModel, EvaluatorInfoViewModel, FlowModel>(
                   (f, ft, e) =>
                   {
                       f.FlowTemplate = ft;
                       f.EvaluatorInfo = e;
                       return f;
                   },
                   "FlowTemplateId, EmployeeId",
                   criteria.GetSpParams());

                return epaFlow;
            }
        }

        public IEnumerable<DepartmentFlowModel> GetDepartmentFlowByFlowTemplateId(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    var flows = multi.Read<DepartmentFlowModel>();
                    var processes = multi.Read<ProcessFlowModel>();

                    GroupProcessFlowByDepartment(flows, processes);

                    return flows;
                }
            }
        }

        public IEnumerable<PersonalFlowModel> GetPersonalFlowByFlowTemplateId(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    var flows = multi.Read<PersonalFlowModel, Group, PersonalFlowModel>(
                        (flow, gr) =>
                        {
                            flow.Group = gr;

                            return flow;
                        },
                        "GroupId");

                    var processes = multi.Read<ProcessFlowModel>();

                    GroupProcessFlowByEmployee(flows, processes);

                    return flows;
                }
            }
        }

        private static void GroupProcessFlowByDepartment(IEnumerable<DepartmentFlowModel> flows, IEnumerable<ProcessFlowModel> processes)
        {
            foreach (DepartmentFlowModel flow in flows)
            {
                foreach (ProcessFlowModel process in processes.Where(x => x.DepartmentId.Equals(flow.DepartmentId)))
                {
                    switch (process.ProcessId)
                    {
                        case (int)ProcessType.HumanResource:
                            flow.HumanResources.Add(process);
                            break;

                        case (int)ProcessType.Evaluator:
                            flow.Evaluators.Add(process);
                            break;

                        case (int)ProcessType.Reviewer:
                            flow.Reviewers.Add(process);
                            break;

                        case (int)ProcessType.Management:
                            flow.Managements.Add(process);
                            break;

                        default:
                            continue;
                    }
                }
            }
        }

        private static void GroupProcessFlowByEmployee(IEnumerable<PersonalFlowModel> flows, IEnumerable<ProcessFlowModel> processes)
        {
            foreach (PersonalFlowModel flow in flows)
            {
                foreach (ProcessFlowModel process in processes.Where(x => x.EmployeeId.Equals(flow.EmployeeId)))
                {
                    switch (process.ProcessId)
                    {
                        case (int)ProcessType.HumanResource:
                            flow.HumanResources.Add(process);
                            break;

                        case (int)ProcessType.Evaluator:
                            flow.Evaluators.Add(process);
                            break;

                        case (int)ProcessType.Reviewer:
                            flow.Reviewers.Add(process);
                            break;

                        case (int)ProcessType.Management:
                            flow.Managements.Add(process);
                            break;

                        default:
                            continue;
                    }
                }
            }
        }
    }
}